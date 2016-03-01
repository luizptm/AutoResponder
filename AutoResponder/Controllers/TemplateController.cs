using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;

namespace AutoResponder.Controllers
{
	[TimeAuthorize]
	//[UserRoleAuthorize(UserRole.Administrator)]
	public class TemplateController : ApplicationController<BR_AutoResponder_TemplateVM>
    {
        private Entities db = new Entities();

        //
        // GET: /Template/

        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_AutoResponder_Template.Include(b => b.BR_AutoResponder_SendingList).ToList();
			IEnumerable<BR_AutoResponder_TemplateVM> list = Mapper.Map<IEnumerable<BR_AutoResponder_TemplateVM>>(dbList);
            return View(list);
        }

        //
        // GET: /Template/Details/5

        public ActionResult Details(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_TemplateVM vm = Mapper.Map<BR_AutoResponder_TemplateVM>(br_autoresponder_template);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
                return Json(new { Status = 0, Message = "Not found" });
            }
			BR_AutoResponder_TemplateVM vm = Mapper.Map<BR_AutoResponder_TemplateVM>(br_autoresponder_template);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

		// GET: /MainSite/VerTemplate/
		[HttpGet]
		public PartialViewResult VerTemplate(int id = 0)
        {
            BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
				ViewBag.Body = "";
                return PartialView("VerTemplate");
            }
			ViewBag.Body = br_autoresponder_template.Body;
			return PartialView("VerTemplate");
        }

        //
        // GET: /Template/Create

        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");
            return View();
        }

        //
        // POST: /Template/Create

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_AutoResponder_TemplateVM vm)
        {
			vm.CREATION_DATE = DateTime.Now;
            if (ModelState.IsValid)
            {
                ViewBag.Message = "";
                if (vm.Sequence < 0)
                {
                    ViewBag.Message = "A sequência é inválida. Somente números maiores que 0.";
                }
                else
                {
                    BR_AutoResponder_Template br_autoresponder_template = Mapper.Map<BR_AutoResponder_Template>(vm);
                    br_autoresponder_template.CREATION_DATE = DateTime.Now;
                    db.BR_AutoResponder_Template.Add(br_autoresponder_template);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name", vm.SendingListId);
            return View(vm);
        }

        //
        // GET: /Template/Edit/5

        public ActionResult Edit(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_TemplateVM vm = Mapper.Map<BR_AutoResponder_TemplateVM>(br_autoresponder_template);
            ViewBag.SendingListId = db.BR_AutoResponder_SendingList;
            return View(vm);
        }

        //
        // POST: /Template/Edit/5

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_AutoResponder_TemplateVM vm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "";
                if (vm.Sequence < 0)
                {
                    ViewBag.Message = "A sequência é inválida. Somente números maiores que 0.";
                }
                else
                {
                    BR_AutoResponder_Template br_autoresponder_template = Mapper.Map<BR_AutoResponder_Template>(vm);
                    db.Entry(br_autoresponder_template).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.SendingListId = db.BR_AutoResponder_SendingList;
            return View(vm);
        }

        //
        // POST: /Template/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_AutoResponder_Template item = db.BR_AutoResponder_Template.Find(id);
			if (item != null)
			{
				db.BR_AutoResponder_Template.Remove(item);
				return db.SaveChanges().ToString();
			}
			else
			{
				return "0";
			}
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

		//http://stackoverflow.com/questions/6750116/how-to-eliminate-all-line-breaks-in-string
		public string RemoveLineEndings(string value)
		{
			if(String.IsNullOrEmpty(value))
			{
				return value;
			}
			string lineSeparator = ((char) 0x2028).ToString();
			string paragraphSeparator = ((char)0x2029).ToString();

			return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
		}
    }
}