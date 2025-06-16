using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniAccounting.Data
{
    public class VoucherRepository
    {
        private readonly string _connectionString;

        public VoucherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void SaveVoucher(string voucherType, DateTime voucherDate, string referenceNo,
                                string createdBy, DataTable voucherDetailsTable)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveVoucher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@VoucherType", voucherType);
                    cmd.Parameters.AddWithValue("@VoucherDate", voucherDate);
                    cmd.Parameters.AddWithValue("@ReferenceNo", referenceNo);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@VoucherDetails", voucherDetailsTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "VoucherDetailsType"; // This must match your user-defined table type

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
