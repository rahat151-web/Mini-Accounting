using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniAccounting.Models;
using System.Text.Json;


namespace MiniAccounting.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get user info
                string userQuery = @"
                SELECT Id, PasswordHash 
                FROM AspNetUsers 
                WHERE UserName = @UserName";

                SqlCommand cmd = new SqlCommand(userQuery, con);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    ModelState.AddModelError("UserName", "UserName does not exists");
                    return View(model);
                }

                string userId = reader["Id"].ToString();
                string passwordHash = reader["PasswordHash"].ToString();
                reader.Close();

                // Verify password
                var hasher = new PasswordHasher<string>();
                var result = hasher.VerifyHashedPassword(null, passwordHash, model.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("Password", "Wrong password.");
                    return View(model);
                }

                // Get RoleId
                string roleQuery = @"
                SELECT RoleId 
                FROM AspNetUserRoles 
                WHERE UserId = @UserId";

                SqlCommand roleCmd = new SqlCommand(roleQuery, con);
                roleCmd.Parameters.AddWithValue("@UserId", userId);
                string roleId = roleCmd.ExecuteScalar().ToString();

                // Get RoleName
                string roleNameQuery = @"SELECT Name FROM AspNetRoles WHERE Id = @RoleId";
                SqlCommand roleNameCmd = new SqlCommand(roleNameQuery, con);
                roleNameCmd.Parameters.AddWithValue("@RoleId", roleId);
                string roleName = roleNameCmd.ExecuteScalar().ToString();

                // Get Permissions
                string permissionQuery = @"
                SELECT m.ModuleId, m.ModuleName, rp.AccessValue 
                FROM RolePermissions rp 
                INNER JOIN Modules m ON rp.ModuleId = m.ModuleId
                WHERE rp.RoleId = @RoleId";

                SqlCommand permissionCmd = new SqlCommand(permissionQuery, con);
                permissionCmd.Parameters.AddWithValue("@RoleId", roleId);
                SqlDataReader permReader = permissionCmd.ExecuteReader();

                List<RolePermission> permissions = new List<RolePermission>();

                while (permReader.Read())
                {
                    permissions.Add(new RolePermission
                    {
                        ModuleId = (int)permReader["ModuleId"],
                        ModuleName = permReader["ModuleName"].ToString(),
                        AccessValue = (int)permReader["AccessValue"]
                    });
                }
                permReader.Close();

                // Store to Session
                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("RoleId", roleId);
                HttpContext.Session.SetString("RoleName", roleName);
                HttpContext.Session.SetString("Permissions", JsonSerializer.Serialize(permissions));

                return RedirectToAction("Index", "Accounts");
            }

         
    }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
