using AutoMapper;
using AutoResponder.Library;
using AutoResponder.Library.Exchange;
using AutoResponder.Library.Helper;
using AutoResponder.Library.Main;
using AutoResponder.Library.Security;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;
using Grid.Mvc.Ajax.GridExtensions;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AutoResponder.Controllers
{
    public class MainSiteController : BaseController
    {
        private Entities db = new Entities();

		private const int defaultPageSize = 10;

		#region Actions

		// GET: /MainSite/AccessDenied
		[HttpGet]
		public ActionResult AccessDenied()
        {
            return View();
        }

		// GET: /MainSite/
		[HttpGet]
        public ActionResult Index()
        {
			AccessControl ac = new AccessControl();
			string result = ac.HasAccess("Administrator, User");
			if (result == "-1")
			{
				ModelState.AddModelError("error", AutoResponder.Resources.Resources.SessionExpired);
				return RedirectToAction("Index", "Login");
			}
			else if (result == "-2")
			{
				ModelState.AddModelError("error", AutoResponder.Resources.Resources.NoAccess);
				return RedirectToAction("Index", "Login");
			}
			ViewBag.SendingLists = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");

            return View();
        }

		#endregion

		#region Ajax GET Methods

		// GET: /MainSite/GetListas/
		[HttpGet]
		public String GetListas()
        {
			string texto = "";
            texto += "<option value=''>-- " + AutoResponder.Resources.Resources.Select + " --</option>";
			List<BR_AutoResponder_SendingList> list = db.BR_AutoResponder_SendingList.ToList();
			foreach(BR_AutoResponder_SendingList item in list)
			{
					texto += "<option value='" + item.Id + "'>" + item.Name + "</option>";
			}
            return texto;
        }

		// GET: /MainSite/GetTagsInDropDownList/
		[HttpGet]
		public String GetTagsInDropDownList()
        {
			string texto = "";
			texto += "<option value=''>-- " + AutoResponder.Resources.Resources.Select + " --</option>";
			List<BR_AutoResponder_Tag> list = db.BR_AutoResponder_Tag.ToList();
			foreach(BR_AutoResponder_Tag item in list)
			{
					texto += "<option value='" + item.Id + "'>" + item.Name + "</option>";
			}
            return texto;
        }

		// GET: /MainSite/GetTags/
		[HttpGet]
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
		
		// GET: /MainSite/Logs/
		[HttpGet]
        public PartialViewResult Logs()
        {
			string data = System.IO.File.ReadAllText(HttpContext.Server.MapPath("/logs/logfileSendings.txt"), System.Text.Encoding.UTF8);
			if (String.IsNullOrEmpty(data))
			{
				data = AutoResponder.Resources.Resources.NoDataToShow;
			}
			else
			{
				data = data.Replace("]", "]<br/>");
			}
            return PartialView("Logs", data);
        }

		// GET: /MainSite/Members/
		[HttpGet]
		public ActionResult Members(int? page)
		{
			var pageIndex = (page ?? 1) - 1; //MembershipProvider expects a 0 for the first page
			var pageSize = 10;
			int totalUserCount; // will be set by call to GetAllUsers due to _out_ paramter :-|

			MembershipUserCollection users = Membership.GetAllUsers(pageIndex, pageSize, out totalUserCount);
			List<MembershipUser> subset = new List<MembershipUser>();
			//List<MembershipUser> subset = (List<MembershipUser>)users;
			StaticPagedList<MembershipUser> usersAsIPagedList = new StaticPagedList<MembershipUser>(subset, pageIndex + 1, pageSize, totalUserCount);

			ViewBag.OnePageOfUsers = usersAsIPagedList;
			return View();
		}

        // GET: /MainSite/SelectCulture/
        [HttpGet]
		public PartialViewResult SelectCulture()
		{
            return PartialView("SelectCulture");
		}

		// GET: /MainSite/ListDetails/
		[HttpGet]
		public PartialViewResult ListDetails(string SendingList)
		{
			if (!String.IsNullOrEmpty(SendingList))
			{
				int id = Int32.Parse(SendingList);
				BR_AutoResponder_SendingList list = db.BR_AutoResponder_SendingList.Find(id);
				ViewBag.Details = list.Description;
				//ViewBag.Tag = list.BR_AutoResponder_Tag.Name;
				ViewBag.Tag = null;
				ViewBag.Tags = list.Tags;
				ViewBag.CreationDate = list.CREATION_DATE;
			}
			else
			{
				ViewBag.Details = "";
				ViewBag.Tag = "";
				ViewBag.CreationDate = "";
			}
			return PartialView("ListDetails");
		}

		// GET: /MainSite/UserDetails/
		[HttpGet]
		public PartialViewResult UserDetails(int id)
		{
			BR_Users br_users = db.BR_Users.Find(id);
            if (br_users == null)
            {
                return PartialView();
            }
			BR_UsersVM vm = Mapper.Map<BR_UsersVM>(br_users);
            return PartialView(vm);
		}

		// GET: /MainSite/Usuarios/
		[HttpGet]
		public PartialViewResult Usuarios(string SendingList, int? page, string grid_column = "", string grid_dir = "", string user_name = "")
		{
			int currentPageIndex = page.HasValue ? page.Value : 1;

			user_name = user_name == "undefined" ? "" : user_name;

			grid_column = HttpContext.Request["grid-column"] != null ? HttpContext.Request["grid-column"].ToString() : grid_column;
			grid_dir = HttpContext.Request["grid-dir"] != null ? HttpContext.Request["grid-dir"].ToString() : grid_dir;

			ViewBag.SendingList = SendingList;
			ViewBag.grid_column = grid_column;
			ViewBag.grid_dir = grid_dir;
			ViewBag.CurrentFilter = user_name;

			List<UsuariosVM> list = new List<UsuariosVM>();
			if (!String.IsNullOrEmpty(SendingList))
			{
				DataAccess da = new DataAccess();
				list = da.Usuarios(SendingList, currentPageIndex, defaultPageSize, grid_column, grid_dir, user_name);
				ViewBag.Sql_Usuarios = da.Sql_Usuarios;
			}
			else
			{
				ViewBag.Sql_Usuarios = "";
			}
			//return CallAjaxGrid_Usuarios(list.AsQueryable(), currentPageIndex, defaultPageSize);
			PagedList<UsuariosVM> paged_list = new PagedList<UsuariosVM>(list, currentPageIndex, defaultPageSize);
			return PartialView(paged_list);
		}

		// GET: /MainSite/Templates/
		[HttpGet]
		public PartialViewResult Templates(string SendingList, int? page)
		{
			int currentPageIndex = page.HasValue ? page.Value : 1;

			ViewBag.SendingList = SendingList;
			
			List<TemplatesVM> list = new List<TemplatesVM>();
			if (!String.IsNullOrEmpty(SendingList))
			{
				DataAccess da = new DataAccess();
				list = da.Templates(SendingList, page);
				ViewBag.Sql_Templates = da.Sql_Templates;
			}
			else
			{
				ViewBag.Sql_Templates = "";
			}
			PagedList<TemplatesVM> paged_list = new PagedList<TemplatesVM>(list, currentPageIndex, defaultPageSize);
			return PartialView(paged_list);
		}

		// GET: /MainSite/Envios/
		[HttpGet]
		public PartialViewResult Envios(string SendingList, int? page, string grid_column = "", string grid_dir = "", string user_name = "", string Open = "", string Unsubscribe = "", string Bounce = "", string Click = "")
		{
			int currentPageIndex = page.HasValue ? page.Value : 1;

			user_name = user_name == "undefined" ? "" : user_name;

			grid_column = HttpContext.Request["grid-column"] != null ? HttpContext.Request["grid-column"].ToString() : grid_column;
			grid_dir = HttpContext.Request["grid-dir"] != null ? HttpContext.Request["grid-dir"].ToString() : grid_dir;

			ViewBag.Open = Open == "1" ? "checked=checked" : "";
			ViewBag.Unsubscribe = Unsubscribe == "1" ? "checked=checked" : "";
			ViewBag.Bounce = Bounce == "1" ? "checked=checked" : "";
			ViewBag.Click = Click == "1" ? "checked=checked" : "";

			ViewBag.SendingList = SendingList;
			ViewBag.grid_column = grid_column;
			ViewBag.grid_dir = grid_dir;
			ViewBag.pOpen = Open;
			ViewBag.pUnsubscribe = Unsubscribe;
			ViewBag.pBounce = Bounce;
			ViewBag.pClick = Click;

			List<EnviosVM> list = new List<EnviosVM>();
			if (!String.IsNullOrEmpty(SendingList))
			{
				DataAccess da = new DataAccess();
				list = da.Envios(SendingList, page, grid_column, grid_dir, user_name, Open, Unsubscribe, Bounce, Click);
				ViewBag.Sql_Envios = da.Sql_Envios;
			}
			else
			{
				ViewBag.Sql_Envios = "";
			}
			//return CallAjaxGrid_Envios(list.AsQueryable(), currentPageIndex, defaultPageSize);
			PagedList<EnviosVM> paged_list = new PagedList<EnviosVM>(list, currentPageIndex, defaultPageSize);
			return PartialView(paged_list);
		}

		// GET: /MainSite/CreateSendingList/
		[HttpGet]
		public PartialViewResult CreateSendingList()
        {
			ViewBag.TagId = new SelectList(db.BR_AutoResponder_Tag, "Id", "Name");
			return PartialView("CreateSendingListPage");
        }

		// GET: /MainSite/CreateTag/
		[HttpGet]
        public PartialViewResult CreateTag()
        {
            return PartialView("CreateTag");
        }
		
		// GET: /MainSite/CreateTemplate/
		[HttpGet]
        public PartialViewResult CreateTemplate(int listId)
        {
			ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");
			List<BR_AutoResponder_Template> templatesByList = db.BR_AutoResponder_Template.Where(x => x.SendingListId == listId).ToList();
			BR_AutoResponder_TemplateVM vm = new BR_AutoResponder_TemplateVM();
			if (templatesByList.Count == 0)
			{
				Stats stats = new Stats();
				stats.SaveOptinDefaultTemplate(listId.ToString());
				BR_AutoResponder_Template templateOptin = db.BR_AutoResponder_Template.Where(x => x.SendingListId == listId && x.Sequence == 0).SingleOrDefault();
				if (templateOptin != null)
				{
					vm = Mapper.Map<BR_AutoResponder_TemplateVM>(templateOptin);
				}
				else
				{
				}
			}
			else
			{
				int sequence = templatesByList.Count - 1;
				vm.Sequence = sequence;
				BR_AutoResponder_Template template = db.BR_AutoResponder_Template.Where(x => x.SendingListId == listId && x.Sequence == sequence).SingleOrDefault();
				if (template != null)
				{
					vm = Mapper.Map<BR_AutoResponder_TemplateVM>(template);
				}
			}
            return PartialView("CreateTemplatePage", vm);
        }
				
		// GET: /MainSite/VerTemplate/
		[HttpGet]
		public PartialViewResult VerTemplate(int id = 0)
        {
            BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
				return PartialView("PartialViewError");
            }
			ViewBag.Body = br_autoresponder_template.Body;
			return PartialView("VerTemplate");
        }

		// GET: /MainSite/EditTemplate/5
		[HttpGet, ValidateInput(false)]
        public PartialViewResult EditTemplate(int id = 0)
        {
			ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");
			BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template == null)
            {
				return PartialView("PartialViewError");
            }
			BR_AutoResponder_TemplateVM vm = Mapper.Map<BR_AutoResponder_TemplateVM>(br_autoresponder_template);
            return PartialView("EditTemplatePage", vm);
        }

		// GET: /MainSite/TestSendingTemplate/
		[HttpGet]
		public PartialViewResult TestSendingTemplate(string SendingList, int id = 0)
        {
			BR_AutoResponder_TemplateVM vm;
            BR_AutoResponder_Template br_autoresponder_template = db.BR_AutoResponder_Template.Find(id);
            if (br_autoresponder_template != null)
            {
				br_autoresponder_template.Sender = Constants.DEFAULT_SENDER_EMAIL;
				DataAccess da = new DataAccess();
				ViewBag.UserId = da.getUsersByList(SendingList);
				vm = Mapper.Map<BR_AutoResponder_TemplateVM>(br_autoresponder_template);
                return PartialView("TestSendingTemplate", vm);
            }
			return PartialView("PartialViewError");
        }

		// GET: /MainSite/ExecutarEnvio/
		[HttpGet]
		public String ExecutarEnvio(int SendingListId = 0, int UserId = 0)
		{
			String output = "";
			SendingScheduledEmail sendingScheduledEmail = new SendingScheduledEmail();
			if (HttpContext.Request.IsLocal)
			{
				output = sendingScheduledEmail.ExecuteTask(SendingListId, UserId);
			}
			else
			{
				output = sendingScheduledEmail.ExecuteTask();
			}
			try
			{
				string outputb = LerBounces();
				if (outputb.StartsWith("-ERR"))
				{
					output += "<br/> " + AutoResponder.Resources.Resources.ErrorReadingBounces + ":" + outputb;
				}
				else
				{
					output += "<br/>" + outputb;
				}
			}
			catch (Exception ex)
			{
				output += "<br/> " + AutoResponder.Resources.Resources.ErrorReadingBounces + ":" + ex.Message;
			}
			return output;
		}

		// GET: /MainSite/ExecuteSending/
		[HttpGet]
		public ActionResult ExecuteSending(int SendingListId = 0, int UserId = 0)
		{
			String output = "";
			SendingScheduledEmail sendingScheduledEmail = new SendingScheduledEmail();
			if (HttpContext.Request.IsLocal)
			{
				output = sendingScheduledEmail.ExecuteTask(SendingListId, UserId);
			}
			else
			{
				output = sendingScheduledEmail.ExecuteTask();
			}
			return View("ExecuteSending", (object)output);
		}

		// GET: /MainSite/LerBounces/
		[HttpGet]
		public String LerBounces()
		{
			if (Constants.USE_POP3 == "true")
			{
				String result = Pop3ReadAccount.LerBounces();
				//String result = Pop3MailReadAccount.LerBounces();
				return result;
			}
			else
			{
				ExchangeServiceMailReader s = new ExchangeServiceMailReader();
				String totalBounces = s.GetBounces();
				return totalBounces;
			}
		}

		// GET: /MainSite/AddUserInList/
		[HttpGet]
		public PartialViewResult AddUserInList(int id)
		{
			ViewBag.SendingListId = new SelectList(db.BR_AutoResponder_SendingList, "Id", "Name");
            ViewBag.UserId = new SelectList(db.BR_Users.Where(x => x.firstName != null && x.firstName != "").OrderBy(y => y.firstName), "idUser", "firstName");
			BR_AutoResponder_UserEntryVM vm = new BR_AutoResponder_UserEntryVM();
			vm.EntryDate = DateTime.Now;
			vm.SendingListId = id;
			return PartialView("AddUserInList", vm);
		}

		#endregion
		
		#region Grix Ajax Methods

		public PartialViewResult CallAjaxGrid_Usuarios(IQueryable<UsuariosVM> data, int? page, int pagePartitionSize = 0)
		{
			var ajaxGridFactory = new AjaxGridFactory();
			AjaxGrid<UsuariosVM> grid;
			if (page.Value == 1)
			{
				grid = (AjaxGrid<UsuariosVM>)ajaxGridFactory.CreateAjaxGrid<UsuariosVM>(data, 1, false, defaultPageSize);
			}
			else
			{
				grid = (AjaxGrid<UsuariosVM>)ajaxGridFactory.CreateAjaxGrid<UsuariosVM>(data, page.HasValue ? page.Value : 1, page.HasValue, defaultPageSize);
			}
			
			//grid.EmptyGridText = "grid vazio";
			//return Json(new { Html = grid.ToJson("Usuarios", this), grid.HasItems }, JsonRequestBehavior.AllowGet);
			//return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Usuarios", grid) }, JsonRequestBehavior.AllowGet);
			return PartialView("Usuarios", grid);
		}

		public PartialViewResult CallAjaxGrid_Envios(IQueryable<EnviosVM> data, int? page, int pagePartitionSize = 0)
		{
			var ajaxGridFactory = new AjaxGridFactory();
			AjaxGrid<EnviosVM> grid;
			if (page.Value == 1)
			{
				grid = (AjaxGrid<EnviosVM>)ajaxGridFactory.CreateAjaxGrid<EnviosVM>(data, 1, false, defaultPageSize);
			}
			else
			{
				grid = (AjaxGrid<EnviosVM>)ajaxGridFactory.CreateAjaxGrid<EnviosVM>(data, page.HasValue ? page.Value : 1, page.HasValue, defaultPageSize);
			}

			//grid.EmptyGridText = "grid vazio";
			//return Json(new { Html = grid.ToJson("Envios", this), grid.HasItems }, JsonRequestBehavior.AllowGet);
			//return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("Envios", grid) }, JsonRequestBehavior.AllowGet);
			return PartialView("Envios", grid);
		}

		#endregion
	}
}
