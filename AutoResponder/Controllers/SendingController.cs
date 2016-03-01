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
    public class SendingController : ApplicationController<BR_AutoResponder_SendingVM>
    {
        private Entities db = new Entities();

		public List<SelectListItem> getRange()
		{
			List<SelectListItem> rangeList = new List<SelectListItem>();
			rangeList.Add(new SelectListItem { Text = "-- selecione --", Value = "" });
			rangeList.Add(new SelectListItem { Text = "Sim", Value = "true" });
			rangeList.Add(new SelectListItem { Text = "Não", Value = "false" });
			return rangeList;
		}

        //
        // GET: /Sending/

        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_AutoResponder_Sending.Include(b => b.BR_AutoResponder_Template).ToList();
			IEnumerable<BR_AutoResponder_SendingVM> list = Mapper.Map<IEnumerable<BR_AutoResponder_SendingVM>>(dbList);
            return View(list);
        }

        //
        // GET: /Sending/Details/5

        public ActionResult Details(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Sending br_autoresponder_sending = db.BR_AutoResponder_Sending.Find(id);
            if (br_autoresponder_sending == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_SendingVM vm = Mapper.Map<BR_AutoResponder_SendingVM>(br_autoresponder_sending);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_AutoResponder_Sending br_autoresponder_sending = db.BR_AutoResponder_Sending.Find(id);
            if (br_autoresponder_sending == null)
            {
                return Json(new { Status = 0, Message = "Not found" });
            }
			BR_AutoResponder_SendingVM vm = Mapper.Map<BR_AutoResponder_SendingVM>(br_autoresponder_sending);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

        //
        // GET: /Sending/Create

        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            ViewBag.UserId = new SelectList(db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName), "idUser", "firstName");
            ViewBag.TemplateId = new SelectList(db.BR_AutoResponder_Template, "Id", "Subject");
			ViewBag.Range = getRange();
            return View();
        }

        //
        // POST: /Sending/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_AutoResponder_SendingVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_AutoResponder_Sending br_autoresponder_sending = Mapper.Map<BR_AutoResponder_Sending>(vm);
				br_autoresponder_sending.CREATION_DATE = DateTime.Now;
                db.BR_AutoResponder_Sending.Add(br_autoresponder_sending);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName), "idUser", "firstName", vm.UserId);
            ViewBag.TemplateId = new SelectList(db.BR_AutoResponder_Template, "Id", "Subject", vm.TemplateId);
			ViewBag.Range = getRange();
            return View(vm);
        }

        //
        // GET: /Sending/Edit/5

        public ActionResult Edit(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Sending br_autoresponder_sending = db.BR_AutoResponder_Sending.Find(id);
            if (br_autoresponder_sending == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_SendingVM vm = Mapper.Map<BR_AutoResponder_SendingVM>(br_autoresponder_sending);
			ViewBag.UserId = db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName);
            ViewBag.TemplateId = db.BR_AutoResponder_Template;
			ViewBag.Range = getRange();
            return View(vm);
        }

        //
        // POST: /Sending/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_AutoResponder_SendingVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_AutoResponder_Sending br_autoresponder_sending = Mapper.Map<BR_AutoResponder_Sending>(vm);
                db.Entry(br_autoresponder_sending).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName);
            ViewBag.TemplateId = db.BR_AutoResponder_Template;
			ViewBag.Range = getRange();
            return View(vm);
        }

        //
        // POST: /Sending/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
            BR_AutoResponder_Sending item = db.BR_AutoResponder_Sending.Find(id);
			if (item != null)
			{
				db.BR_AutoResponder_Sending.Remove(item);
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
    }
}