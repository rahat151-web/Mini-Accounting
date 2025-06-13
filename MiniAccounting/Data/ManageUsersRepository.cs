using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace MiniAccounting.Data
{
    public class ManageUsersRepository
    {
        private readonly string _connectionString;

        public ManageUsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task ManageUserAsync(string action, IdentityUser user, string roleName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("ManageUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Required parameters
                    command.Parameters.AddWithValue("@Action", action);
                    command.Parameters.AddWithValue("@Id", user.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserName", user.UserName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NormalizedUserName", user.NormalizedUserName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NormalizedEmail", user.NormalizedEmail ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmailConfirmed", user.EmailConfirmed);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SecurityStamp", user.SecurityStamp ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ConcurrencyStamp", user.ConcurrencyStamp ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)user.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumberConfirmed", user.PhoneNumberConfirmed);
                    command.Parameters.AddWithValue("@TwoFactorEnabled", user.TwoFactorEnabled);
                    command.Parameters.AddWithValue("@LockoutEnd", (object)user.LockoutEnd ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LockoutEnabled", user.LockoutEnabled);
                    command.Parameters.AddWithValue("@AccessFailedCount", user.AccessFailedCount);

                    // RoleName (for INSERT/UPDATE), NULL for DELETE
                    command.Parameters.AddWithValue("@RoleName", (object)roleName ?? DBNull.Value);

                    // RoleId parameter is used as local variable inside SP, so we pass null
                    command.Parameters.AddWithValue("@RoleId", DBNull.Value);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> UserExistsAsync(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers WHERE NormalizedUserName = @UserName", connection);
                command.Parameters.AddWithValue("@UserName", userName.ToUpper());

                var count = (int)await command.ExecuteScalarAsync();

                if (count > 0)
                {
                    
                    return true;
                }

                return false;
            }
        }

    }
}
