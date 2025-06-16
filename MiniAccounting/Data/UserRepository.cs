using Microsoft.Data.SqlClient;
using MiniAccounting.Models.Users;

namespace MiniAccounting.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT UserName FROM AspNetUsers WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                    return reader["UserName"].ToString();

                return null;
                

                
            }
        }
    }
}
