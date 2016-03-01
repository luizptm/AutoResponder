using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using AutoMapper;
using AutoResponder.Library;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;

namespace AutoResponder.Controllers
{
	[TimeAuthorize]
	//[UserRoleAuthorize(UserRole.Administrator)]
    public class UsersController : ApplicationController<BR_UsersVM>
    {
        private Entities db = new Entities();

		string TagExplanation = AutoResponder.Resources.Resources.TagExplanation;

		#region users

		//
		// GET: /Users/

        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var dbList = db.BR_Users.ToList();
			IEnumerable<BR_UsersVM> list = Mapper.Map<IEnumerable<BR_UsersVM>>(dbList);
			return View("Index", list);
        }

        //
        // GET: /Users/Details/5

        public ActionResult Details(int id = 0)
        {
            BR_Users br_users = db.BR_Users.Find(id);
            if (br_users == null)
            {
                return HttpNotFound();
            }
			BR_UsersVM vm = Mapper.Map<BR_UsersVM>(br_users);
            return View(vm);
        }

		[HttpPost]
        public JsonResult GetDetails(int id)
        {
			BR_Users br_users = db.BR_Users.Find(id);
            if (br_users == null)
            {
                return Json(new { Status = 0, Message = "Not found" });
            }
			BR_UsersVM vm = Mapper.Map<BR_UsersVM>(br_users);
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", vm) });
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
			ViewBag.TagExplanation = TagExplanation;
            return View();
        }

        //
        // POST: /Users/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BR_UsersVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_Users br_users = Mapper.Map<BR_Users>(vm);
                db.BR_Users.Add(br_users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

			ViewBag.TagExplanation = TagExplanation;
            return View(vm);
        }

        //
        // GET: /Users/Edit/5

        public ActionResult Edit(int id = 0)
        {
            BR_Users br_users = db.BR_Users.Find(id);
            if (br_users == null)
            {
                return HttpNotFound();
            }
			BR_UsersVM vm = Mapper.Map<BR_UsersVM>(br_users);
			ViewBag.TagExplanation = TagExplanation;
            return View(vm);
        }

        //
        // POST: /Users/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BR_UsersVM vm)
        {
			if (ModelState.IsValid)
            {
				BR_Users br_users = Mapper.Map<BR_Users>(vm);
                db.Entry(br_users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
			ViewBag.TagExplanation = TagExplanation;
            return View(vm);
        }

        //
        // GET: /Users/Delete/5

        public ActionResult Delete(int id = 0)
        {
            BR_Users br_users = db.BR_Users.Find(id);
            if (br_users == null)
            {
                return HttpNotFound();
            }
			BR_Users vm = Mapper.Map<BR_Users>(br_users);
            return View(vm);
        }

        //
        // POST: /Users/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_Users item = db.BR_Users.Find(id);
			if (item != null)
			{
				db.BR_Users.Remove(item);
				return db.SaveChanges().ToString();
				//return RedirectToAction("Index");
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

		#endregion

		#region private methods

		public JsonResult GetTagsByJson()
        {
			IList<BR_AutoResponder_Tag> list = new List<BR_AutoResponder_Tag>();

			foreach (var item in db.Database.SqlQuery<BR_AutoResponder_Tag>("SELECT Id, Name FROM BR_AutoResponder_Tag u"))
			{
				list.Add(item);
			}

            return Json(new { Items = list }, JsonRequestBehavior.AllowGet);
        }

		#endregion
	}
}