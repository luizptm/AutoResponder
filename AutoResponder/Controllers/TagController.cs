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

namespace AutoResponder.Controllers.Web
{
	[TimeAuthorize]
	//[UserRoleAuthorize(UserRole.Administrator)]
	public class TagController : ApplicationController<BR_AutoResponder_TagVM>
    {
        private Entities db = new Entities();

        //
        // GET: /Tag/

        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_AutoResponder_Tag.ToList();
			IEnumerable<BR_AutoResponder_TagVM> list = Mapper.Map<IEnumerable<BR_AutoResponder_TagVM>>(dbList);
            return View(list);
        }

        //
        // GET: /Tag/Details/5

        public ActionResult Details(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Tag br_autoresponder_tag = db.BR_AutoResponder_Tag.Find(id);
            if (br_autoresponder_tag == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_TagVM vm = Mapper.Map<BR_AutoResponder_TagVM>(br_autoresponder_tag);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_AutoResponder_Tag br_autoresponder_tag = db.BR_AutoResponder_Tag.Find(id);
            if (br_autoresponder_tag == null)
            {
                return Json(new { Status = 0, Message = "Not found" });
            }
			BR_AutoResponder_TagVM vm = Mapper.Map<BR_AutoResponder_TagVM>(br_autoresponder_tag);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

        //
        // GET: /Tag/Create

        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            return View();
        }

        //
        // POST: /Tag/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_AutoResponder_TagVM vm)
        {
			ViewBag.Error = "";
            if (ModelState.IsValid)
            {
				string nameTag = vm.Name.Trim();
				if (db.BR_AutoResponder_Tag.Where(x => x.Name.Trim() == nameTag).Count() <= 0)
				{
					BR_AutoResponder_Tag br_autoresponder_tag = Mapper.Map<BR_AutoResponder_Tag>(vm);
					br_autoresponder_tag.CREATION_DATE = DateTime.Now;
					db.BR_AutoResponder_Tag.Add(br_autoresponder_tag);
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
        // GET: /Tag/Edit/5

        public ActionResult Edit(int id = 0)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            BR_AutoResponder_Tag br_autoresponder_tag = db.BR_AutoResponder_Tag.Find(id);
            if (br_autoresponder_tag == null)
            {
                return HttpNotFound();
            }
			BR_AutoResponder_TagVM vm = Mapper.Map<BR_AutoResponder_TagVM>(br_autoresponder_tag);
            return View(vm);
        }

        //
        // POST: /Tag/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_AutoResponder_TagVM vm)
        {
			ViewBag.Error = "";
            if (ModelState.IsValid)
            {
				string nameTag = vm.Name.Trim();
				int idTag = vm.Id;
				if (db.BR_AutoResponder_Tag.Where(x => x.Name.Trim() == nameTag && x.Id != idTag).Count() <= 0)
				{
					BR_AutoResponder_Tag br_autoresponder_tag = Mapper.Map<BR_AutoResponder_Tag>(vm);
					db.Entry(br_autoresponder_tag).State = EntityState.Modified;
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
        // POST: /Tag/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_AutoResponder_Tag item = db.BR_AutoResponder_Tag.Find(id);
			if (item != null)
			{
				db.BR_AutoResponder_Tag.Remove(item);
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

		//
        // GET: /Tag/GetList

        public String GetList()
        {
			var output = "";
			List<BR_AutoResponder_Tag> list = db.BR_AutoResponder_Tag.ToList<BR_AutoResponder_Tag>();
			foreach (BR_AutoResponder_Tag tag in list)
			{
				output += tag.Name.ToLower() + " ";
			}
			output = output.Substring(0, output.Length - 1);
            return output;
        }
    }
}