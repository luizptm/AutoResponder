using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;

namespace AutoResponder.Controllers
{
	[TimeAuthorize]
	//[UserRoleAuthorize(UserRole.Administrator)]
	public class AccessUsersController : ApplicationController<BR_AccessControl_User>
    {
        private AC_Entities db = new AC_Entities();

        // GET: /Users/
        public async Task<ActionResult> Index()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            var br_accesscontrol_user = db.BR_AccessControl_User.Include(b => b.BR_AccessControl_Role);
            return View(await br_accesscontrol_user.ToListAsync());
        }

        // GET: /Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BR_AccessControl_User br_accesscontrol_user = await db.BR_AccessControl_User.FindAsync(id);
            if (br_accesscontrol_user == null)
            {
                return HttpNotFound();
            }
            return View(br_accesscontrol_user);
        }

		[HttpPost]
		public JsonResult GetDetails(int id)
		{
			BR_AccessControl_User users = db.BR_AccessControl_User.Find(id);
			if (users == null)
			{
				return Json(new { Status = 0, Message = "Not found" });
			}
			return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Details", users) });
		}

        // GET: /Users/Create
        public ActionResult Create()
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            ViewBag.RoleId = new SelectList(db.BR_AccessControl_Role, "Id", "Name");
            return View();
        }

        // POST: /Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Name,Trigram,Password,RoleId,CREATION_DATE")] BR_AccessControl_User br_accesscontrol_user)
        {
            if (ModelState.IsValid)
            {
				br_accesscontrol_user.CREATION_DATE = DateTime.Now;
                db.BR_AccessControl_User.Add(br_accesscontrol_user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.BR_AccessControl_Role, "Id", "Name", br_accesscontrol_user.RoleId);
            return View(br_accesscontrol_user);
        }

        // GET: /Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
			AccessControl ac = new AccessControl();
			if (ac.GetUser("Administrator") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BR_AccessControl_User br_accesscontrol_user = await db.BR_AccessControl_User.FindAsync(id);
            if (br_accesscontrol_user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.BR_AccessControl_Role, "Id", "Name", br_accesscontrol_user.RoleId);
            return View(br_accesscontrol_user);
        }

        // POST: /Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Name,Trigram,Password,RoleId,CREATION_DATE")] BR_AccessControl_User br_accesscontrol_user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(br_accesscontrol_user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.BR_AccessControl_Role, "Id", "Name", br_accesscontrol_user.RoleId);
            return View(br_accesscontrol_user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public String DeleteConfirmed(int id)
        {
			BR_AccessControl_User item = db.BR_AccessControl_User.Find(id);
			if (item != null)
			{
				db.BR_AccessControl_User.Remove(item);
				return db.SaveChanges().ToString();
			}
			else
			{
				return "0";
			}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
