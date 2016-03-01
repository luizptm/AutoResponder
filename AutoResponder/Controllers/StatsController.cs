using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoResponder.Library;
using AutoResponder.Web.Models.Entity;

namespace AutoResponder.Controllers
{
    public class StatsController : Controller
    {
		private Entities db = new Entities();

        // GET: /Stats/
		[HttpGet]
        public ActionResult Index()
        {
			String url = HttpContext.Request.RawUrl.ToLower();

			Stats stats = new Stats();
			String result = stats.RecordStat(url);

			string hide = "";
			if (!HttpContext.Request.IsLocal)
			{
				hide = "visibility:hidden";
			}
			result = result.Replace("--hide--", hide);

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
