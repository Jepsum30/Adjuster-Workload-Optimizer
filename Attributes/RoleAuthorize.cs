using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdjusterOptimizerAPI.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            // Check if user is logged in
            var role = httpContext.Session.GetString("ROLE");

            if (role == null)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "You must be logged in to access this resource."
                });
                return;
            }

            // Check if user has the required role
            if (!string.Equals(role, _requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
