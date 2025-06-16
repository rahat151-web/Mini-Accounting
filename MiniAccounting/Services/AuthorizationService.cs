using MiniAccounting.Models;
using System.Text.Json;

namespace MiniAccounting.Services
{
    public class AuthorizationService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthorizationService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool HasPermission(string moduleName, int accessValue)
        {
            var json = _contextAccessor.HttpContext.Session.GetString("Permissions");
            if (string.IsNullOrEmpty(json)) return false;

            var permissions = JsonSerializer.Deserialize<List<RolePermission>>(json);

            return permissions.Any(p => p.ModuleName == moduleName && p.AccessValue >= accessValue);
        }
    }
}
