using System.Web.Mvc;

namespace AutoResponder.Controllers
{
    public class SpamCheckController : Controller
    {
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}
    }
}
