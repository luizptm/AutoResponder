using AutoMapper;
using AutoResponder.Library;
using AutoResponder.Web.Library.Main;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AutoResponder.Controllers
{
	//[UserRoleAuthorize(UserRole.Administrator)]
    public class MainSitePostController : BaseController
    {
		private Entities db = new Entities();
		
		#region Ajax POST Methods

		// POST: /MainSitePost/CreateSendingList/
		[HttpPost]
        [ValidateAntiForgeryToken]
		public JsonResult CreateSendingList(BR_AutoResponder_SendingListVM vm)
        {
            if (ModelState.IsValid)
            {
				string nameList = vm.Name.Trim();
				List<BR_AutoResponder_SendingList> sendingLists = db.BR_AutoResponder_SendingList.Where(x => x.Name.Trim() == nameList).ToList();
				if (sendingLists.Count > 0)
				{
					return Json(new { Status = 0, Message = AutoResponder.Resources.Resources.ErrorCreatingSendingList, Content = "" });
				}
				else
				{
					BR_AutoResponder_SendingList br_autoresponder_sendinglist = Mapper.Map<BR_AutoResponder_SendingList>(vm);
					br_autoresponder_sendinglist.Interval = 0;
					br_autoresponder_sendinglist.CREATION_DATE = DateTime.Now;
					db.BR_AutoResponder_SendingList.Add(br_autoresponder_sendinglist);
					db.SaveChanges();

					Stats stats = new Stats();
					stats.SaveOptinDefaultTemplate(br_autoresponder_sendinglist.Id.ToString());
					return Json(new { Status = 1, Message = "Ok", Content = "" });
				}
            }
			else
			{
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
        }

		// POST: /MainSitePost/CreateTag/
		[HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTag(BR_AutoResponder_TagVM vm)
        {
			ViewBag.Error = "";
            if (ModelState.IsValid)
            {
				string nameTag = vm.Name.Trim();
				if (db.BR_AutoResponder_Tag.Where(x => x.Name.Trim() == nameTag).Count() > 0)
				{
					return Json(new { Status = 0, Message = AutoResponder.Resources.Resources.ErrorCreatingTag, Content = "" });
				}
				else
				{
					BR_AutoResponder_Tag br_autoresponder_tag = Mapper.Map<BR_AutoResponder_Tag>(vm);
					br_autoresponder_tag.CREATION_DATE = DateTime.Now;
					db.BR_AutoResponder_Tag.Add(br_autoresponder_tag);
					db.SaveChanges();
					return Json(new { Status = 1, Message = "Ok", Content = "" });
				}
            }
			else
			{
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
        }

		// POST: /MainSitePost/CreateTemplate/
		[HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTemplate(BR_AutoResponder_TemplateVM vm)
        {
            if (ModelState.IsValid)
            {
				if (vm.Sequence < 0)
				{
					return Json(new { Status = 0, Message = AutoResponder.Resources.Resources.ErrorCreatingTemplate, Content = "" });
				}
				else
				{
					if (!String.IsNullOrEmpty(vm.RedirectURL))
					{
						vm.Body = vm.Body.Replace("<!--!LINK_OPTIN!-->", "http://" + vm.RedirectURL);
					}
					BR_AutoResponder_Template br_autoresponder_template = Mapper.Map<BR_AutoResponder_Template>(vm);
					br_autoresponder_template.CREATION_DATE = DateTime.Now;
					db.BR_AutoResponder_Template.Add(br_autoresponder_template);
					db.SaveChanges();
					return Json(new { Status = 1, Message = "Ok", Content = "" });
				}
            }
			else
			{
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
        }

		// POST: /MainSitePost/EditTemplate/5
		[HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult EditTemplate(BR_AutoResponder_TemplateVM vm)
        {
            if (ModelState.IsValid)
            {
				if (vm.Sequence < 0)
				{
					return Json(new { Status = 0, Message = AutoResponder.Resources.Resources.ErrorCreatingSendingList, Content = "" });
				}
				else
				{
					BR_AutoResponder_Template br_autoresponder_template = Mapper.Map<BR_AutoResponder_Template>(vm);
					if (br_autoresponder_template.Sequence == 0)
					{
						br_autoresponder_template.Interval = 0;
					}
					db.Entry(br_autoresponder_template).State = EntityState.Modified;
					db.SaveChanges();
					return Json(new { Status = 1, Message = "Ok", Content = "" });
				}
            }
			else
			{
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
        }

		// POST: /MainSitePost/DeleteTemplate/5
		[HttpPost]
		public JsonResult DeleteTemplate(int id)
		{
			BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            db.BR_AutoResponder_Template.Remove(br_autoresponder_template);
            db.SaveChanges();
            return Json(new { Status = 1, Message = "Ok", Content = "" });
		}

		// POST: /MainSitePost/TestSendingTemplate
		[HttpPost, ValidateInput(false)]
		public JsonResult TestSendingTemplate(BR_AutoResponder_TemplateVM vm)
		{
			if (!String.IsNullOrEmpty(vm.Sender) && !String.IsNullOrEmpty(vm.UserId) && !String.IsNullOrEmpty(vm.Subject))
			{
				if (vm.Sequence == 0)
				{
					Stats stats = new Stats();
					bool result = stats.SendOptinTemplate(vm.UserId.ToString(), vm.SendingListId.ToString());
					if (result)
					{
						return Json(new { Status = 1, Message = "Ok", Content = "" });
					}
					else
					{
						return Json(new { Status = 0, Message = String.Format(AutoResponder.Resources.Resources.ErrorSendingEmailWithSequence, 0), Content = "" });
					}
				}
				else
				{
					int idUser = Int32.Parse(vm.UserId);
					BR_Users user = db.BR_Users.Where(x => x.idUser == idUser).SingleOrDefault();
					if (user != null)
					{
						string body = Stats.InsertStats(vm.Body, vm.Id.ToString(), vm.UserId);
						string username = SendingScheduledEmail.getUserFullName(user);

						SendingScheduledEmail s = new SendingScheduledEmail();
						bool result = s.SendMail(vm.Subject, body, username, user.email, idUser.ToString(), vm.Id.ToString(), vm.SendingListId.ToString());
						if (result)
						{
							return Json(new { Status = 1, Message = "Ok", Content = "" });
						}
						else
						{
							return Json(new { Status = 0, 
                                Message = String.Format(AutoResponder.Resources.Resources.ErrorSendingEmailWithSequence, vm.Sequence.ToString()), 
                                Content = "" });
						}
					}
					else
					{
						return Json(new { Status = 0, Message = AutoResponder.Resources.Resources.ErrorUserNotExists, Content = "" });
					}
				}
			}
			else
			{
				if (String.IsNullOrEmpty(vm.UserId))
				{
					ViewBag.Error += AutoResponder.Resources.Resources.ErrorRecipientRequired + "<br/>";
				}
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
		}

		// POST: /MainSitePost/AddUserInList/5
		[HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult AddUserInList(BR_AutoResponder_UserEntryVM vm)
        {
            if (ModelState.IsValid)
            {
				BR_AutoResponder_UserEntry br_autoresponder_userentry = Mapper.Map<BR_AutoResponder_UserEntry>(vm);
				br_autoresponder_userentry.CREATION_DATE = DateTime.Now;
				db.BR_AutoResponder_UserEntry.Add(br_autoresponder_userentry);
				db.SaveChanges();
				return Json(new { Status = 1, Message = "Ok", Content = "" });
            }
			else
			{
				List<ModelError> modelStateErrors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors).ToList<ModelError>();
				foreach (ModelError e in modelStateErrors) {
					ViewBag.Error += e.ErrorMessage + "<br/>";
				}
				return Json(new { Status = 0, Message = ViewBag.Error, Content = "" });
			}
        }

		// POST: /MainSite/DetectSPAM/
		[HttpPost, ValidateInput(false)]
		public JsonResult DetectSPAM(string text)
		{
			text = HtmlRemoval.StripTagsRegex(text);
			text = HtmlRemoval.StripTagsCharArray(text);
			text = text.Replace("clique neste link", "");

			string result = SpamScore.Get(text, false);
			if (result.ToLower().StartsWith("é spam"))
			{
				return Json(new {IsSpam = true, Result = result});
			}
			else if (result.ToLower().StartsWith("não é spam"))
			{
				return Json(new { IsSpam = false, Result = result });
			}
			else if (result.ToLower().StartsWith("spam"))
			{
				return Json(new { IsSpam = true, Result = result });
			}
			else if (result.ToLower().StartsWith("nospam"))
			{
				return Json(new { IsSpam = false, Result = result });
			}
			return Json(new { IsSpam = false, Result = "ERRO" });
		}

		#endregion
	}
}
 