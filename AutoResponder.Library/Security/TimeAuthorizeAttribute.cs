using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AutoResponder.Library.Security
{
    public class TimeAuthorizeAttribute : AuthorizeAttribute
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }

		public string RedirectActionName { get; set; }
		public string RedirectControllerName { get; set; }

		public TimeAuthorizeAttribute()
		{
			StartTime = 9;
			EndTime = 19;
		}

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
			if (DateTime.Now.Hour < StartTime)
			{
				return false;
			}

            if (EndTime <= DateTime.Now.Hour)
			{
				return false;
			}

            return true;
        }

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

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
}