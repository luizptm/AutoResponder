using AutoMapper;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AutoResponder.Controllers
{
	[TimeAuthorize]
	//[UserRoleAuthorize(UserRole.Administrator)]
    public class SendingListController : ApplicationController<BR_AutoResponder_SendingListVM>
    {
        private Entities db = new Entities();

		string TagExplanation = AutoResponder.Resources.Resources.TagExplanation;

        //
        // GET: /SendingList/

        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_AutoResponder_SendingList.ToList();
			IEnumerable<BR_AutoResponder_SendingListVM> list = Mapper.Map<IEnumerable<BR_AutoResponder_SendingListVM>>(dbList);
            return View(list);
        }

        //
        // GET: /SendingList/Details/5

        public ActionResult Details(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_SendingList br_autoresponder_sendinglist = db.BR_AutoResponder_SendingList.Find(id);
            if (br_autoresponder_sendinglist == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_SendingListVM vm = Mapper.Map<BR_AutoResponder_SendingListVM>(br_autoresponder_sendinglist);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_AutoResponder_SendingList br_autoresponder_sendinglist = db.BR_AutoResponder_SendingList.Find(id);
            if (br_autoresponder_sendinglist == null)
            {
                return Json(new { Status = 0, Message = "Not found" });
            }
			BR_AutoResponder_SendingListVM vm = Mapper.Map<BR_AutoResponder_SendingListVM>(br_autoresponder_sendinglist);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

        //
        // GET: /SendingList/Create

        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			ViewBag.TagExplanation = TagExplanation;
            return View();
        }

        //
        // POST: /SendingList/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_AutoResponder_SendingListVM vm)
        {
            if (ModelState.IsValid)
            {
				string name = vm.Name.Trim();
				if (db.BR_AutoResponder_SendingList.Where(x => x.Name.Trim() == name).Count() <= 0)
				{
					BR_AutoResponder_SendingList br_autoresponder_sendinglist = Mapper.Map<BR_AutoResponder_SendingList>(vm);
					br_autoresponder_sendinglist.Interval = 0;
					br_autoresponder_sendinglist.CREATION_DATE = DateTime.Now;
					db.BR_AutoResponder_SendingList.Add(br_autoresponder_sendinglist);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					ViewBag.Error = "Esta tag já está cadastrada.";
				}
            }
            return View(vm);
        }

        //
        // GET: /SendingList/Edit/5

        public ActionResult Edit(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_SendingList br_autoresponder_sendinglist = db.BR_AutoResponder_SendingList.Find(id);
            if (br_autoresponder_sendinglist == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_SendingListVM vm = Mapper.Map<BR_AutoResponder_SendingListVM>(br_autoresponder_sendinglist);
			ViewBag.TagExplanation = TagExplanation;
            return View(vm);
        }

        //
        // POST: /SendingList/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_AutoResponder_SendingListVM vm)
        {
            if (ModelState.IsValid)
            {
				string name = vm.Name.Trim();
				int id = vm.Id;
				if (db.BR_AutoResponder_SendingList.Where(x => x.Name.Trim() == name && x.Id != id).Count() <= 0)
				{
					BR_AutoResponder_SendingList br_autoresponder_sendinglist = Mapper.Map<BR_AutoResponder_SendingList>(vm);
					db.Entry(br_autoresponder_sendinglist).State = EntityState.Modified;
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					ViewBag.Error = "Esta tag já está cadastrada.";
				}
            }
            ViewBag.TagId = db.BR_AutoResponder_Tag;
            return View(vm);
        }

        //
        // POST: /SendingList/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_AutoResponder_SendingList item = db.BR_AutoResponder_SendingList.Find(id);
			if (item != null)
			{
				db.BR_AutoResponder_SendingList.Remove(item);
				return db.SaveChanges().ToString();
			}
			else
			{
				return "0";
			}
        }

		//
		// GET: /Tag/GetTags

		public String GetTags()
		{
			var output = "";
			List<BR_AutoResponder_Tag> list = db.BR_AutoResponder_Tag.ToList<BR_AutoResponder_Tag>();
			foreach (BR_AutoResponder_Tag tag in list)
			{
				output += tag.Name.ToLower() + ",";
			}
			if (output.Length > 0)
			{
				output = output.Substring(0, output.Length - 1);
			}
			return output;
		}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}