using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdjusterOptimizerAPI.Attributes
{
    /// <summary>
    /// Custom authorization attribute that restricts access
    /// to users whose session role matches one of the allowed roles.
    /// </summary>
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
            var httpContext = context.HttpContext;

            // Check login status
            var role = httpContext.Session.GetString("ROLE");

            if (role == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "You must be logged in to access this resource."
                });
                return;
            }

            // Check if user has ANY allowed role
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
