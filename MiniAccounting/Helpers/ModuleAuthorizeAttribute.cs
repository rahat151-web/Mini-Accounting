using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniAccounting.Services;

namespace MiniAccounting.Helpers
{
    public class ModuleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly int _accessValue;

        public ModuleAuthorizeAttribute(string moduleName, int accessValue)
        {
            _moduleName = moduleName;
            _accessValue = accessValue;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            var userId = httpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Not logged in
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var authService = httpContext.RequestServices.GetService<AuthorizationService>();

            if (authService == null || !authService.HasPermission(_moduleName, _accessValue))
            {
                // Redirect to Access Denied page
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }
        }
    }
}
