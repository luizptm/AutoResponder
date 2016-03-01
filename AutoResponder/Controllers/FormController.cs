using System;
using System.Net;
using System.Web.Mvc;
using AutoResponder.Library;

namespace AutoResponder.Controllers
{
    public class FormController : Controller
    {
        //
        // GET: /Form/
        public ActionResult Index()
        {
            return View();
        }

		//
		// GET: /Form/Error
		/// <summary>
		/// Error
		/// </summary>
		/// <returns></returns>
		public ActionResult Error()
		{
			return View();
		}

		//
		// GET: /Form/Success
		/// <summary>
		/// Success
		/// </summary>
		/// <returns></returns>
		public ActionResult Success()
		{
			ViewBag.Success = "Dados cadastrados com sucesso";
			return View();
		}

		//
		// POST: /Form/
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(UserVM model)
		{
			String nome = model.Nome;
			String email = model.Email;
			if (!String.IsNullOrEmpty(nome) && !String.IsNullOrEmpty(email))
			{
				String responseSaveData = SaveUserData.Save(model);
				if (responseSaveData.Contains("OK"))
				{
					String returnURL = Request.Form["returnURL"] == null ? "" : Request.Form["returnURL"];
					if (returnURL != "")
					{
						Response.Redirect(returnURL);
					}
					//ViewBag.Success = "Dados cadastrados com sucesso";
					//return View("Success");
					return RedirectToAction("Success");
				}
				else
				{
					ViewBag.Error = responseSaveData;
					return View("Error");
				}
			}
			ModelState.AddModelError("error", "Os campos nome e e-mail são obrigatórios");
			return View();
		}

		//
		// POST: /Form/
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public String Post(UserVM model)
		{
			String responseSaveData = SaveUserData.Save(model);
			return responseSaveData;
		}
	}
}