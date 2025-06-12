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
                throw new RepositoryException("Failed to add account: " + ex.Message, ex);
            }




        }

        public void UpdateAccount(Accounts account)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_ManageChartOfAccounts", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
            cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            cmd.Parameters.AddWithValue("@AccountName", account.AccountName);
            cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId);
            cmd.Parameters.AddWithValue("@AccountType", account.AccountType);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteAccount(Accounts account)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_ManageChartOfAccounts", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
            cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            cmd.Parameters.AddWithValue("@AccountName", account.AccountName);
            cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId);
            cmd.Parameters.AddWithValue("@AccountType", account.AccountType);

            con.Open();
            cmd.ExecuteNonQuery();
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


    }
}
