using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using AutoResponder.Models.Entity;
using AutoResponder.ViewModels;

namespace AutoResponder.Library
{
	public class DataAccess
	{
		private Entities db = new Entities();

		public string Sql_Usuarios = "";
		public string Sql_Templates = "";
		public string Sql_Envios = "";

		public List<UsuariosVM> Usuarios(string SendingList, int pageNumber, int rowsPerPage, string grid_column = "", string grid_dir = "", string user_name = "")
		{
			int id = Int32.Parse(SendingList);
			string tags = "";
			BR_AutoResponder_SendingList sendingList = db.BR_AutoResponder_SendingList.Find(id);
			if (sendingList != null)
			{
				//BR_AutoResponder_Tag tag = sendingList.BR_AutoResponder_Tag;
				tags = sendingList.Tags;
			}

			List<UsuariosVM> list = new List<UsuariosVM>();

				string filter_user = "";
				if (user_name != "")
				{
					filter_user = "AND (u.firstName LIKE '%" + user_name + "%' OR u.middleName LIKE '%" + user_name + "%' OR u.lastName LIKE '%" + user_name + "%' OR u.email LIKE '%" + user_name + "%') ";
				}
				
				// *** ATTENTION  ***
				//COLLATE Latin1_General_BIN  - not works in 'INTSQLBRA'
				// *** ATTENTION  ***
				/*
				sql = "SELECT u.idUser, u.firstName, u.middleName, u.lastName, u.email, getdate(), 0, 0, 0, 0 " + 
				"FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY idUser) AS RowNum FROM BR_Users) AS u " + 
				"WHERE RowNum BETWEEN ((@PageNumber-1)*@RowsPerPage)+1 AND @RowsPerPage*(@PageNumber)";//DEBUG
				sql = sql.Replace("@PageNumber", pageNumber.ToString());
				sql = sql.Replace("@RowsPerPage", rowsPerPage.ToString());
				*/
				grid_column = grid_column == "FullName" ? "u.firstName" : grid_column;
				grid_column = grid_column == "Email" ? "u.email" : grid_column;
				grid_column = grid_column == "EntryDate" ? "ue.EntryDate" : grid_column;
				grid_column = grid_column == "Unsubscribe" ? "Unsubscribe" : grid_column;
				grid_column = grid_column == "OpenMail" ? "OpenMail" : grid_column;
				grid_column = grid_column == "Bounce" ? "Bounce" : grid_column;
				grid_column = grid_column == "Click" ? "Click" : grid_column;
				if (grid_column == "") grid_column = "u.firstName";
				grid_dir = grid_dir == "" || grid_dir == "0" ? "ASC" : "DESC";

				var sqlEntities = db.Database.SqlQuery<UsuariosCustomQueryVM>(
					"SELECT " +
						"u.idUser, u.firstName, u.middleName, u.lastName, u.email, ue.EntryDate, " +
						"(SELECT SUM(CAST(s.Unsubscribe AS INT))FROM dbo.BR_AutoResponder_Sending s " +
							"INNER JOIN dbo.BR_AutoResponder_Template t ON s.TemplateId = t.Id " +
							"WHERE t.SendingListId = '" + SendingList + "' AND s.UserId = u.idUser) AS UNSUBSCRIBE, " +
						"(SELECT SUM(CAST(s.OpenMail AS INT)) FROM dbo.BR_AutoResponder_Sending s " +
							"INNER JOIN dbo.BR_AutoResponder_Template t ON s.TemplateId = t.Id " +
							"WHERE t.SendingListId = '" + SendingList + "' AND s.UserId = u.idUser) AS OPENMAIL, " +
						"(SELECT SUM(CAST(s.Bounce AS INT)) FROM dbo.BR_AutoResponder_Sending s " +
							"INNER JOIN dbo.BR_AutoResponder_Template t ON s.TemplateId = t.Id " +
							"WHERE t.SendingListId = '" + SendingList + "' AND s.UserId = u.idUser) AS BOUNCE, " +
						"(SELECT SUM(CAST(s.Click AS INT)) FROM dbo.BR_AutoResponder_Sending s " +
							"INNER JOIN dbo.BR_AutoResponder_Template t ON s.TemplateId = t.Id " +
							"WHERE t.SendingListId = '" + SendingList + "' AND s.UserId = u.idUser) AS CLICK " +
					"FROM dbo.BR_AutoResponder_Sending s " +
					"RIGHT OUTER JOIN dbo.BR_Users u ON s.UserId = u.idUser " +
					"INNER JOIN dbo.BR_AutoResponder_UserEntry ue ON ue.UserId = u.idUser " +
					"WHERE ue.SendingListId = '" + SendingList + "' " + filter_user +
					"GROUP BY u.idUser, u.firstName, u.middleName, u.lastName, u.email, ue.EntryDate " +
					"ORDER BY " + grid_column + " " + grid_dir);
				
				this.Sql_Usuarios = sqlEntities.ToString();
				
				foreach (var item in sqlEntities)
				{
					UsuariosVM u = new UsuariosVM();
					u.idUser = item.idUser;//Int32.Parse(item.idUser);
					u.firstName = item.firstName;
					u.middleName = item.middleName;
					u.lastName = item.lastName;
					u.FullName = string.Format("{0}{1}{2}", u.firstName, !String.IsNullOrEmpty(u.middleName) ? " " + u.middleName : "", !String.IsNullOrEmpty(u.lastName) ? " " + u.lastName : "");
					u.Email = item.email;
					u.EntryDate = item.entryDate;
					u.Unsubscribe = item.UNSUBSCRIBE.HasValue ? item.UNSUBSCRIBE.Value : 0;
					u.OpenMail = item.OPENMAIL.HasValue ? item.OPENMAIL.Value : 0;
					u.Bounce = item.BOUNCE.HasValue ? item.BOUNCE.Value : 0;
					u.Click = item.CLICK.HasValue ? item.CLICK.Value : 0;
					list.Add(u);
				}

			return list;
		}

		public List<TemplatesVM> Templates(string SendingList, int? page)
		{
			int id = Int32.Parse(SendingList);
			
			List<TemplatesVM> list = new List<TemplatesVM>();
			
			var sqlEntities = db.Database.SqlQuery<TemplatesCustomQueryVM>(
				"SELECT t.Id AS TemplateId, t.Sequence, t.Interval, t.Sender, t.Subject, " +
				"SUM(CAST(s.Unsubscribe AS INT)) AS Unsubscribe, " +
				"SUM(CAST(s.OpenMail AS INT)) AS OpenMail, " +
				"SUM(CAST(s.Bounce AS INT)) AS Bounce, " +
				"SUM(CAST(s.Click AS INT)) AS Click, " +
				"COUNT(CAST(s.Id AS INT)) AS Sendings " +
				"FROM dbo.BR_AutoResponder_Template t " +
				"INNER JOIN dbo.BR_AutoResponder_SendingList l ON t.SendingListid = l.Id " +
				"LEFT OUTER JOIN dbo.BR_AutoResponder_Sending s ON s.TemplateId = t.Id " +
				"WHERE l.Id = " + id + " " +
				"GROUP BY t.Id, t.Sequence, t.Interval, t.Sender, t.Subject ORDER BY t.Sequence ASC");

			this.Sql_Templates = sqlEntities.ToString();

			foreach (TemplatesCustomQueryVM item in sqlEntities)
			{
				TemplatesVM t = new TemplatesVM();
				t.TemplateId = item.TemplateId;
				t.Sequence = item.Sequence;
				t.Interval = item.Interval;
				t.Sender = item.Sender;
				t.Subject = item.Subject;
				t.Unsubscribe = item.Unsubscribe.HasValue ? item.Unsubscribe.Value : 0;
				t.OpenMail = item.OpenMail.HasValue ? item.OpenMail.Value : 0;
				t.Bounce = item.Bounce.HasValue ? item.Bounce.Value : 0;
				t.Click = item.Click.HasValue ? item.Click.Value : 0;
				t.Sendings = item.Sendings;
				list.Add(t);
			}

			return list;
		}

		public List<EnviosVM> Envios(string SendingList, int? page, string grid_column = "", string grid_dir = "", string user_name = "", string Open = "", string Unsubscribe = "", string Bounce = "", string Click = "")
		{
			int id = Int32.Parse(SendingList);

			List<EnviosVM> list = new List<EnviosVM>();

			string filter_user = "";
			if (user_name != "")
			{
				filter_user = "AND (u.firstName LIKE '%" + user_name + "%' OR u.middleName LIKE '%" + user_name + "%' OR u.lastName LIKE '%" + user_name + "%' OR u.email LIKE '%" + user_name + "%') ";
			}

			string filter_type = "";
			if (Open == "1")
			{
				filter_type += "AND s.OpenMail = " + Open + " ";
			}
			if (Unsubscribe == "1")
			{
				filter_type += "AND s.Unsubscribe = " + Unsubscribe + " ";
			}
			if (Bounce == "1")
			{
				filter_type += "AND s.Bounce = " + Bounce + " ";
			}
			if (Click == "1")
			{
				filter_type += "AND s.Click = " + Click + " ";
			}

			grid_column = grid_column == "FullName" ? "u.firstName" : grid_column;
			grid_column = grid_column == "Subject" ? "t.Subject" : grid_column;
			grid_column = grid_column == "Email" ? "u.email" : grid_column;
			grid_column = grid_column == "SentDate" ? "s.SentDate" : grid_column;
			grid_column = grid_column == "Unsubscribe" ? "Unsubscribe" : grid_column;
			grid_column = grid_column == "OpenMail" ? "OpenMail" : grid_column;
			grid_column = grid_column == "Bounce" ? "Bounce" : grid_column;
			grid_column = grid_column == "Click" ? "Click" : grid_column;
			if (grid_column == "") grid_column = "u.firstName";
			grid_dir = grid_dir == "" || grid_dir == "0" ? "ASC" : "DESC";

			var sqlEntities = db.Database.SqlQuery<EnviosCustomQueryVM>(
				"SELECT CAST(s.Unsubscribe AS INT) AS Unsubscribe, CAST(s.OpenMail AS INT) AS OpenMail, CAST(s.Bounce AS INT) AS Bounce, CAST(s.Click AS INT) AS Click, " +
				"s.SentDate, u.firstName, u.middleName, u.lastName, t.Id AS TemplateId, t.Subject, u.email AS Email  " +
				"FROM dbo.BR_AutoResponder_Sending s, dbo.BR_Users u, dbo.BR_AutoResponder_Template t, dbo.BR_AutoResponder_SendingList l " +
				"WHERE s.UserId = u.idUser AND t.SendingListid = l.Id AND t.Id = s.TemplateId AND l.Id = " + id + " " + filter_user + " " + filter_type + " " +
				"ORDER BY " + grid_column + " " + grid_dir);

			this.Sql_Envios = sqlEntities.ToString();

			foreach (var item in sqlEntities)
			{
				EnviosVM e = new EnviosVM();
				e.Unsubscribe = item.Unsubscribe.HasValue ? item.Unsubscribe.Value : 0;
				e.OpenMail = item.OpenMail.HasValue ? item.OpenMail.Value : 0;
				e.Bounce = item.Bounce.HasValue ? item.Bounce.Value : 0;
				e.Click = item.Click.HasValue ? item.Click.Value : 0;
				e.SentDate = item.SentDate;
				string firstName = item.firstName;
				string middleName = item.middleName;
				string lastName = item.lastName;
				e.FullName = string.Format("{0}{1}{2}", firstName, !String.IsNullOrEmpty(middleName) ? " " + middleName : "", !String.IsNullOrEmpty(lastName) ? " " + lastName : "");
				e.TemplateId = item.TemplateId;
				e.Subject = item.Subject;
				e.Email = item.Email;
				list.Add(e);
			}

			return list;
		}

		public List<SelectListItem> getUsersByList(string SendingListId)
		{
			List<SelectListItem> list = new List<SelectListItem>();

			foreach (var item in db.Database.SqlQuery<UsuariosCustomQueryVM>(
				"SELECT u.idUser, u.firstName, u.middleName, u.lastName, u.email " +
				"FROM BR_Users u " +
				"INNER JOIN BR_AutoResponder_UserEntry ue ON ue.UserId = u.idUser " +
				"WHERE ue.SendingListId = '" + SendingListId + "' "))
			{
				UsuariosVM u = new UsuariosVM();
				u.idUser = item.idUser;
				u.firstName = item.firstName;
				u.middleName = item.middleName;
				u.lastName = item.lastName;
				u.FullName = string.Format("{0}{1}{2}", u.firstName, !String.IsNullOrEmpty(u.middleName) ? " " + u.middleName : "", !String.IsNullOrEmpty(u.lastName) ? " " + u.lastName : "");
				u.Email = item.email;

				SelectListItem itemSelectList = new SelectListItem { Value = u.idUser.ToString(), Text = u.FullName + " (" + u.Email + ")" };
				list.Add(itemSelectList);
			}

			return list;
		}

		/// <summary>
		/// getUsers
		/// </summary>
		/// <param name="PageNumber"></param>
		/// <param name="RowsPerPage"></param>
		/// <returns></returns>
		public List<SelectListItem> getUsers(int PageNumber = 0, int RowsPerPage = 0)
		{
			List<SelectListItem> list = new List<SelectListItem>();
			string sql = "";
			if (PageNumber != 0 && RowsPerPage != 0)
			{
				int n1 = ((PageNumber - 1) * RowsPerPage) + 1;
				int n2 = RowsPerPage * (PageNumber);
				sql = "SELECT u.idUser, u.firstName, u.middleName, u.lastName, u.email FROM BR_Users u WHERE RowNum  BETWEEN " + n1 + " AND " + n2;
			}
			else
			{
				sql = "SELECT u.idUser, u.firstName, u.middleName, u.lastName, u.email FROM BR_Users u";
			}

			foreach (var item in db.Database.SqlQuery<UsuariosCustomQueryVM>(sql))
			{
				UsuariosVM u = new UsuariosVM();
				u.idUser = item.idUser;
				u.firstName = item.firstName;
				u.middleName = item.middleName;
				u.lastName = item.lastName;
				u.FullName = string.Format("{0}{1}{2}", u.firstName, !String.IsNullOrEmpty(u.middleName) ? " " + u.middleName : "", !String.IsNullOrEmpty(u.lastName) ? " " + u.lastName : "");
				u.Email = item.email;

				SelectListItem itemSelectList = new SelectListItem { Value = u.idUser.ToString(), Text = u.FullName + " (" + u.Email + ")" };
				list.Add(itemSelectList);
			}

			return list;
		}
	}
}