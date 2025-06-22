using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MiniAccounting.Models;
using MiniAccounting.Models.Users;
using System;
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

        public async Task CreateUserAsync(string username, string email, string password, string phoneNumber, string roleName)
        {
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await ManageUserAsync("INSERT", user, roleName);
        }

        public async Task UpdateUserAsync(string Id, string email, string password, string phoneNumber, string roleName)
        {
            var user = new IdentityUser
            {
                Id = Id,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            if (password == null)
            {
                user.PasswordHash = null;
            }

            else
            {
                var passwordHasher = new PasswordHasher<IdentityUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, password);
            }

            await ManageUserAsync("UPDATE", user, roleName);



            
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
                "ON b.RoleId = c.Id "+
                "WHERE c.Name <> 'Admin'", con);

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

        public async Task<bool> hasVoucherAsync(string Id)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("select a.UserName "+
            "from AspNetUsers a Inner Join Vouchers v "+
            "ON a.Id = v.CreatedBy "+
            "where a.Id = @UserId", con);

            cmd.Parameters.AddWithValue("@UserId", Id);

            await con.OpenAsync();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return true;



            

            return false;

        }

        public async Task DeleteUserAsync(string Id)
        {
            var user = new IdentityUser
            {
                Id = Id
            };

            await ManageUserAsync("DELETE", user, null);




        }



    }
}
