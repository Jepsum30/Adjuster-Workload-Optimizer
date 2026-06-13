using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AdjusterOptimizerAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _requiredRoles;

        public RoleAuthorizeAttribute(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Not logged in
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "You must be logged in to access this resource."
                });
                return;
            }

            // Extract role from claims
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (role == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "User role not found."
                });
                return;
            }

            // Check if role is allowed
            bool authorized = _requiredRoles
                .Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

            if (!authorized)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
