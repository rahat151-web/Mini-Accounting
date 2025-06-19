using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using MiniAccounting.Exceptions;
using MiniAccounting.Models;
using System.Data;

namespace MiniAccounting.Data
{
    public class AccountsRepository
    {
        private readonly string _connectionString;

        public AccountsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddAccount(Accounts account)
        {
            try
            {

                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode);
                cmd.Parameters.AddWithValue("@AccountName", account.AccountName);
                cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId);
                cmd.Parameters.AddWithValue("@AccountType", account.AccountType);

                con.Open();
                cmd.ExecuteNonQuery();

            }

            catch (SqlException ex)
            {
                throw new RepositoryException("An error occurred while saving voucher: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while saving voucher.", ex);
            }




        }

        public void UpdateAccount(Accounts account)
        {
            try
            {
                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
                cmd.Parameters.AddWithValue("@AccountName", account.AccountName);

                con.Open();
                cmd.ExecuteNonQuery();

            }

            catch (SqlException ex)
            {
                throw new RepositoryException("An error occurred while saving voucher: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while saving voucher.", ex);
            }


        }

        public void DeleteAccount(Accounts account)
        {
            try
            {
                using SqlConnection con = new(_connectionString);
                using SqlCommand cmd = new("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@AccountId", account.AccountId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                throw new RepositoryException("An error occurred while saving voucher: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while saving voucher.", ex);
            }

        }

        public List<Accounts> GetAccountsByParent(int? parentAccountId)
        {
            List<Accounts> accounts = new();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetAccountsByParent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)parentAccountId ?? DBNull.Value);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new Accounts
                {
                    AccountId = reader.GetInt32(0),
                    AccountCode = reader.GetString(1),
                    AccountName = reader.GetString(2),
                    ParentAccountId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    AccountType = reader.GetString(4),
                    IsLeaf = reader.GetInt32(5),
                    HasVoucher = reader.GetInt32(6)
                });
            }

            return accounts;
        }

        public List<string> GetLeafAccounts()
        {
            var accounts = new List<string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var sql = @"
            SELECT A.AccountCode 
            FROM Accounts A
            WHERE NOT EXISTS (
                SELECT 1 FROM Accounts C WHERE C.ParentAccountId = A.AccountId
            )";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return accounts;
        }

        public int GetAccountId(string code)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                connection.Open();

                var command = new SqlCommand("SELECT AccountId FROM Accounts WHERE AccountCode = @AcctCode", connection);
                command.Parameters.AddWithValue("@AcctCode", code);

                using SqlDataReader reader =  command.ExecuteReader();

                if (reader.Read())
                    return reader.GetInt32(0);

                return -5;



            }

        }

        public Accounts GetAccountDetails(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                connection.Open();

                var command = new SqlCommand("SELECT AccountId, AccountCode, AccountName, AccountType FROM Accounts WHERE AccountId = @AcctId", connection);
                command.Parameters.AddWithValue("@AcctId", id);

                using SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Accounts
                    {
                        AccountId = reader.GetInt32(0),
                        AccountCode = reader.GetString(1),
                        AccountName = reader.GetString(2),
                        AccountType = reader.GetString(3)
                       
                    };
                }
                    

                return null;



            }

        }





    }
}
