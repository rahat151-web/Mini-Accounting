using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MiniAccounting.Models;
using MiniAccounting.Models.Users;
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

        public async Task<List<UserModel>> GetUserDetailsAsync()
        {
            List<UserModel> allUsers = new();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("select a.Id, a.UserName, a.Email, c.Name, a.PhoneNumber " +
                "From AspNetUsers a Inner JOIN AspNetUserRoles b " +
                "ON a.Id = b.UserId " +
                "Inner JOIN AspNetRoles c " +
                "ON b.RoleId = c.Id;", con);

                await con.OpenAsync();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allUsers.Add(new UserModel
                    { 
                        Id = reader.GetString(0),
                        UserName = reader.GetString(1),
                        Email = reader.GetString(2),
                        RoleName = reader.GetString(3),
                        PhoneNumber = reader.GetString(4)


                    });
                }

                return allUsers;
            
        }

        public async Task<UserModel> GetUserDataAsync(string Id)
        {
            UserModel user = new UserModel();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("select a.Id, a.UserName, a.Email, c.Name, a.PhoneNumber " +
                "From AspNetUsers a Inner JOIN AspNetUserRoles b " +
                "ON a.Id = b.UserId " +
                "Inner JOIN AspNetRoles c " +
                "ON b.RoleId = c.Id " +
                "WHERE a.Id = @UserId", con);

            cmd.Parameters.AddWithValue("@UserId", Id);

            await con.OpenAsync();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                user.Id = reader.GetString(0);
                user.UserName = reader.GetString(1);
                user.Email = reader.GetString(2);
                user.RoleName = reader.GetString(3);
                user.PhoneNumber = reader.GetString(4);


               
            }

            return user;


        }

    }
}
