using AutoMapper;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AutoResponder.Controllers
{
    public class AdminController : BaseController
    {
        private AC_Entities db = new AC_Entities();

		public ActionResult ChangePassword()
		{
            AccessControl ac = new AccessControl();
			if (ac.GetUser("User") == null)
			{
				return RedirectToAction("Index", "Login");
			}
            
			MembershipUser user = ac.GetUser();
			CustomMembershipUser customUser = (CustomMembershipUser)user;

            LoginChangePassword vm = new LoginChangePassword();
            vm.UserId = customUser != null ? customUser.Id : 0;
            vm.UserName = customUser != null ? customUser.Trigram : "";
			return View(vm);
		}

        [HttpPost]
        public ActionResult ChangePassword(LoginChangePassword vm)
		{
            if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(vm.UserName, vm.OldPassword))
                {
			        ModelState.AddModelError("error", AutoResponder.Resources.Resources.PasswordNotMatchError); 
				}
                else if (vm.NewPassword.Trim().ToLower() == vm.ConfirmPassword.Trim().ToLower())
                {
                    ModelState.AddModelError("error", AutoResponder.Resources.Resources.LoginError); 
                }
                else
                {
                    BR_AccessControl_User br_accesscontrol_user = new BR_AccessControl_User();
                    br_accesscontrol_user = db.BR_AccessControl_User.Find(vm.UserId);                    
                    br_accesscontrol_user.Trigram = vm.UserName;
                    br_accesscontrol_user.Password = vm.NewPassword;
                    db.Entry(br_accesscontrol_user).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    ViewBag.Message = AutoResponder.Resources.Resources.PasswordChangeSucces;
                    return View(vm);
				}
            }
			return View(vm);
		}

        public ActionResult Register()
		{
			return View();
		}

        [HttpPost]
        public ActionResult Register(LoginRegister vm)
		{
            if (ModelState.IsValid)
			{
                List<String> trigrams = db.BR_AccessControl_User.Select<BR_AccessControl_User, string>(x => x.Trigram).ToList<String>();
                if (trigrams.Contains(vm.Trigram.Trim().ToLower()))
                {
                    ModelState.AddModelError("error", AutoResponder.Resources.Resources.UserNameIsAlreadyUsedError); 
                }
                else
                {
			        BR_AccessControl_User br_accesscontrol_user = new BR_AccessControl_User();                 
                    br_accesscontrol_user.Trigram = vm.Trigram;
                    br_accesscontrol_user.Password = vm.Password;
                    br_accesscontrol_user.RoleId = 2; //user
                    br_accesscontrol_user.Name = vm.UserName;
                    br_accesscontrol_user.CREATION_DATE = DateTime.Now;
                    db.Entry(br_accesscontrol_user).State = EntityState.Added;
                    db.SaveChanges();
                    
                    ViewBag.Message = AutoResponder.Resources.Resources.CreateANewAccountSuccess;
                    return View(vm);
                }
            }
			return View(vm);
		}
	}
}