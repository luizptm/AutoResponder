using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoResponder.Library;

namespace AutoResponder.Controllers
{
    public class ConfirmController : Controller
    {
        //
        // GET: /Confirm/
        public ActionResult Index()
        {
            String url = HttpContext.Request.RawUrl.ToLower();

			Stats stats = new Stats();
			String result = stats.SaveOptin(url);
			ViewBag.Result = result;

			if (result.StartsWith("http") || result.StartsWith("www"))
			{
				result = result.StartsWith("www") ? "http://" + result : result;
				Response.Redirect(result);
			}
			else
			{
				ViewBag.Result = result;
			}

			return View();
        }
	}
}