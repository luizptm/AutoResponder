using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AutoResponder.Library.Security
{
    public class UserRoleAuthorizeAttribute : AuthorizeAttribute
    {
		public string RedirectActionName { get; set; }
		public string RedirectControllerName { get; set; }

        public UserRoleAuthorizeAttribute() {}
		
        public UserRoleAuthorizeAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
		
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var accessAllowed = false;

			AccessControl ac = new AccessControl();
			string[] userRoles = ac.GetCurrentUserRoles();

			if (userRoles != null)
			{
				string[] allowedRoles = this.Roles.Split(',');
				foreach (string role in allowedRoles)
				{
					if (userRoles.Contains(role))
					{
						accessAllowed = true;
					}
				}
			}
			return accessAllowed;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.Result is HttpUnauthorizedResult)
			{
				var values = new RouteValueDictionary(new
				{
					action = this.RedirectActionName == string.Empty ? "AccessDenied" : this.RedirectActionName,
					controller = this.RedirectControllerName == string.Empty ? "MainSite" : this.RedirectControllerName
				});

				filterContext.Result = new RedirectToRouteResult(values);
			}
		}
    }

    public enum UserRole
    {
        Administrator = 1,
        User = 2
    }
}