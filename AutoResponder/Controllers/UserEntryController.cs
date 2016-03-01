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
    public class UserEntryController : ApplicationController<BR_AutoResponder_UserEntryVM>
    {
        private Entities db = new Entities();

        //
        // GET: /UserEntry/

        public ActionResult Index()
		{
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_AutoResponder_UserEntry.Include(b => b.BR_AutoResponder_SendingList).Include(b => b.BR_Users).ToList();
			IEnumerable<BR_AutoResponder_UserEntryVM> list = Mapper.Map<IEnumerable<BR_AutoResponder_UserEntryVM>>(dbList);
			return View(list);
		}

        //
        // GET: /UserEntry/Details/5

        public ActionResult Details(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_UserEntry br_autoresponder_userentry = db.BR_AutoResponder_UserEntry.Find(id);
            if (br_autoresponder_userentry == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_UserEntryVM vm = Mapper.Map<BR_AutoResponder_UserEntryVM>(br_autoresponder_userentry);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_AutoResponder_UserEntry br_autoresponder_userentry = db.BR_AutoResponder_UserEntry.Find(id);
			if (br_autoresponder_userentry == null)
			{
				return Json(new { Status = 0, Message = "Not found" });
			}
			BR_AutoResponder_UserEntryVM vm = Mapper.Map<BR_AutoResponder_UserEntryVM>(br_autoresponder_userentry);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

        //
        // GET: /UserEntry/Create

        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");
            ViewBag.UserId = new SelectList(db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName), "idUser", "firstName");
            return View();
        }

        //
        // POST: /UserEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_AutoResponder_UserEntryVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_AutoResponder_UserEntry br_autoresponder_userentry = Mapper.Map<BR_AutoResponder_UserEntry>(vm);
				br_autoresponder_userentry.CREATION_DATE = DateTime.Now;
                db.BR_AutoResponder_UserEntry.Add(br_autoresponder_userentry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name", vm.SendingListId);
            ViewBag.UserId = new SelectList(db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName), "idUser", "firstName", vm.UserId);
            return View(vm);
        }

        //
        // GET: /UserEntry/Edit/5

        public ActionResult Edit(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_UserEntry br_autoresponder_userentry = db.BR_AutoResponder_UserEntry.Find(id);
            if (br_autoresponder_userentry == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_UserEntryVM vm = Mapper.Map<BR_AutoResponder_UserEntryVM>(br_autoresponder_userentry);
            ViewBag.SendingListId = db.BR_AutoResponder_SendingList;
			ViewBag.UserId = db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName);
            return View(vm);
        }

        //
        // POST: /UserEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_AutoResponder_UserEntryVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_AutoResponder_UserEntry br_autoresponder_userentry = Mapper.Map<BR_AutoResponder_UserEntry>(vm);
                db.Entry(br_autoresponder_userentry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SendingListId = db.BR_AutoResponder_SendingList;
            ViewBag.UserId = db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName);
            return View(vm);
        }

        //
        // POST: /UserEntry/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_AutoResponder_UserEntry item = db.BR_AutoResponder_UserEntry.Find(id);
			if (item != null)
			{
				db.BR_AutoResponder_UserEntry.Remove(item);
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