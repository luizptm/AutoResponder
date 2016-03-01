using AutoResponder.Library.Helper;
using AutoResponder.Library.Security;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace AutoResponder.Controllers
{
    public class ApplicationController<T> : BaseController
    {
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
		}

		public void Validate()
		{
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				RedirectToAction("Index", "Login");
			}
		}

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;
         
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe
         
            // Modify current thread's cultures           
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
         
            return base.BeginExecuteCore(callback, state);
        }
		/*
		#region Action

		public void Create()
		{
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				RedirectToAction("Index", "Login");
			}
		}

		public void Edit()
		{
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				RedirectToAction("Index", "Login");
			}
		}

		public void Delete()
		{
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				RedirectToAction("Index", "Login");
			}
		}

		#endregion
		*/
	}
}
