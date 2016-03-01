using System.Web.Mvc;
using System.Web.Security;
using AutoResponder.Library.Security;
using AutoResponder.ViewModels;

namespace AutoResponder.Controllers
{
	public class LoginController : BaseController
	{
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Index(string returnUrl = "")
		{
			if (User.Identity.IsAuthenticated)
			{
				return LogOut();
			}

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Index(Login login, string returnUrl = "")
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(login.UserName, login.Password))
				{
					Session["login"] = login;
					return RedirectToLocal(returnUrl);
				}
				ModelState.AddModelError("error", AutoResponder.Resources.Resources.LoginError);
			}
			return View(login);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult LogOut()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Login", null);
		}

		#region Helpers

		private ActionResult RedirectToLocal(string returnUrl = "")
		{
			if (returnUrl != "" && Url.IsLocalUrl(returnUrl) && !returnUrl.Contains("Login"))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "MainSite");
			}
		}

		#endregion
	}
}