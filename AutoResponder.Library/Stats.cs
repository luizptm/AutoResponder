using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using AutoResponder.Library.Main;
using AutoResponder.Models.Entity;

namespace AutoResponder.Library
{
	public class Stats
	{
		private Entities db = new Entities();

		public static string InsertStats(string body, string idTemplate, string idUser, bool useUnsubscribeLink = true)
		{
			string src = "";
			
			/** CLICK **/
			body = ReplaceTemplateLinks(body, idTemplate, idUser);

			/** OPEN **/
			src = Constants.APP_URL + "/Stats/?o/" + idTemplate + "/" + idUser;
			string open_stats = "<div align='center' id='link_stats_open'><img src='" + src + "' width='1' height='1'/></div>";
			open_stats = WebUtility.HtmlDecode(open_stats);
			open_stats = open_stats.Replace("&amp;", "&");

			string unsubscribe_stats = "";
			if (useUnsubscribeLink)
			{
				/** UNSUBSCRIBE **/
				src = Constants.APP_URL + "/Stats/?u/" + idTemplate + "/" + idUser;
				unsubscribe_stats = "<div align='center' id='link_stats_unsubscribe'><i>Caso queira deixar de receber os meus emails, clique em ";
				unsubscribe_stats += "<a href='" + src + "' width='1' height='1'>cancelar inscrição</a>.</i>";
				unsubscribe_stats += "</div>";
				unsubscribe_stats = WebUtility.HtmlDecode(unsubscribe_stats);
				unsubscribe_stats = unsubscribe_stats.Replace("&amp;", "&");
			}
			
			return body + open_stats + unsubscribe_stats;
		}

		public string RecordStat(string url)
        {
			String Result = "";

			string action = "";
			string idT = "";
			string idU = "";
			string redirect = "";

			url = url.Replace("/stats/?", "");
			url = url.Replace("/Stats/?", "");
			url = url.Replace("https://", "hts:");
			url = url.Replace("http://", "ht:");
			string[] sep = { "/" };
			string[] paramsUrl = url.Split(sep, StringSplitOptions.RemoveEmptyEntries);
			if (paramsUrl.Count() >= 3)
			{
				action = paramsUrl[0];
				idT = paramsUrl[1];
				idU = paramsUrl[2];
				if (paramsUrl.Count() > 3)
				{
					for (int i = 3; i < paramsUrl.Count(); i++)
					{
						redirect += paramsUrl[i] + "/";
					}
					redirect = redirect.Substring(0, redirect.Length - 1);
				}
			}
			int result = 0;
			
			if (String.IsNullOrEmpty(action) || (action.ToLower() != "b" && action.ToLower() != "c" && action.ToLower() != "o" && action.ToLower() != "u"))
			{
				Result = "<div id='result' style='--hide--'>Nenhuma ação especificada ou não-encontrada.</div>";
				return Result;
			}
			if (!String.IsNullOrEmpty(action) && !String.IsNullOrEmpty(idT) && !String.IsNullOrEmpty(idU))
			{
				int idTemplate = 0;
				Int32.TryParse(idT, out idTemplate);
				int idUser = 0;
				Int32.TryParse(idU, out idUser);
				if (idTemplate > 0 && idUser > 0)
				{
					BR_AutoResponder_Sending br_autoresponder_sending = new BR_AutoResponder_Sending();
					br_autoresponder_sending = db.BR_AutoResponder_Sending.Where(x => x.UserId == idUser && x.TemplateId == idTemplate).FirstOrDefault();
					if (br_autoresponder_sending != null)
					{
						if (action == "b") //BOUNCE
						{
							br_autoresponder_sending.Bounce = true;
						}
						else if (action == "c") //CLICK
						{
							br_autoresponder_sending.Click = true;
						}
						else if (action == "o") //OPEN
						{
							br_autoresponder_sending.OpenMail = true;
						}
						else if (action == "u") //UNSUBSCRIBE
						{
							BR_AutoResponder_Template template = db.BR_AutoResponder_Template.Find(idTemplate);
							if (template != null)
							{
								List<BR_AutoResponder_UserEntry> entries = db.BR_AutoResponder_UserEntry.Where(x => x.UserId == idUser && x.SendingListId == template.SendingListId).ToList();
								if (entries != null)
								{
									BR_AutoResponder_UserEntry br_autoresponder_userentry = entries.First();
									br_autoresponder_userentry.Optin = false;
									db.Entry(br_autoresponder_userentry).State = EntityState.Modified;
								}
							}
							
							br_autoresponder_sending.Unsubscribe = true;
						}
						db.Entry(br_autoresponder_sending).State = EntityState.Modified;
						result = db.SaveChanges();
					}
				}
			}
			if (action == "c" && redirect != "") //CLICK
			{
				redirect = redirect.Replace("hts:", "https://");
				redirect = redirect.Replace("ht:", "http://");
				//Response.Redirect(redirect);
				Result = redirect;
			}
			else
			{
				if (result > 0)
				{
					Result = "<div id='result' style='--hide--'>Registro alterado com sucesso.</div>";
				}
				else
				{
					Result = "<div id='result' style='--hide--'>Nenhum registro foi alterado.</div>";
				}
			}
            return Result;
        }

		public void SaveOptinDefaultTemplate(string SendingListId)
		{
			int listId = 0;
			Int32.TryParse(SendingListId, out listId);
			if (listId > 0)
			{
				string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("/Resources/Templates/Optin.html"));
			
				BR_AutoResponder_Template template = new BR_AutoResponder_Template();
				template.SendingListId = listId;
				template.Sequence = 0;
				template.Interval = 0;
				template.Sender = Constants.DEFAULT_SENDER_EMAIL;
				template.Subject = "Campanha";
				template.Body = body;
				template.CREATION_DATE = DateTime.Now;

				db.BR_AutoResponder_Template.Add(template);
				db.SaveChanges();
			}
		}

		public bool SendOptinTemplate(string UserId, string SendingListId)
		{
			bool result = false;

			int listId = 0;
			Int32.TryParse(SendingListId, out listId);
			if (listId > 0)
			{
				BR_AutoResponder_Template template = db.BR_AutoResponder_Template.Where(x => x.SendingListId == listId && x.Sequence == 0).Single();
				if (template != null)
				{
					string templateId = template.Id.ToString();

					int userId = Int32.Parse(UserId);
					BR_Users user = db.BR_Users.Where(x => x.idUser == userId).SingleOrDefault();
					if (user != null)
					{
						string subject = template.Subject;

						string optin_link = Constants.APP_URL + "/Confirm/?" + UserId + "/" + SendingListId;

						string username = SendingScheduledEmail.getUserFullName(user);

						string body = template.Body;
						
						body = body.Replace("<!--!STR_NAME!-->", username);
						body = body.Replace("<!--!LINK_OPTIN!-->", optin_link);
						//insere verificação de OPEN e CLICK no e-mail de Optin
						body = Stats.InsertStats(body, templateId.ToString(), userId.ToString(), false);

						string  email = user.email;
						SendingScheduledEmail s = new SendingScheduledEmail();
						result = s.SendMail(subject, body, username, email, UserId, templateId, SendingListId);
					}
				}
			}
			return result;
		}

		public string SaveOptin(string url)
		{
			String Result = "";

			string idU = "";
			string idL = "";
			string redirect = "";
			
			url = url.Replace("/confirm/?", "");
			url = url.Replace("/Confirm/?", "");
			url = url.Replace("https://", "hts:");
			url = url.Replace("http://", "ht:");
			string[] sep = { "/" };
			string[] paramsUrl = url.Split(sep, StringSplitOptions.RemoveEmptyEntries);
			if (paramsUrl.Count() >= 2)
			{
				idU = paramsUrl[0];
				idL = paramsUrl[1];
				if (paramsUrl.Count() > 2)
				{
					for (int i = 2; i < paramsUrl.Count(); i++)
					{
						redirect += paramsUrl[i] + "/";
					}
					redirect = redirect.Substring(0, redirect.Length - 1);
				}
			}
			int result = 0;
			
			if (!String.IsNullOrEmpty(idU) && !String.IsNullOrEmpty(idL))
			{
				int idUser = 0;
				Int32.TryParse(idU, out idUser);
				int idList = 0;
				Int32.TryParse(idL, out idList);
				if (idUser > 0 && idList > 0)
				{
					List<BR_AutoResponder_UserEntry> userEntriesByUserList = db.BR_AutoResponder_UserEntry.Where(x => x.UserId == idUser && x.SendingListId == idList).ToList();
					BR_AutoResponder_UserEntry userentry = new BR_AutoResponder_UserEntry();
					if (userEntriesByUserList.Count() > 0)
					{
						userentry = userEntriesByUserList.Single();
						userentry.Optin = true;
						db.Entry(userentry).State = EntityState.Modified;
					}
					else
					{
						userentry.UserId = idUser;
						userentry.SendingListId = idList;
						userentry.Optin = true;
						userentry.EntryDate = DateTime.Now;
						userentry.CREATION_DATE = DateTime.Now;
						db.Entry(userentry).State = EntityState.Added;
					}
					result = db.SaveChanges();
				}
			}
			if (redirect != "")
			{
				redirect = redirect.Replace("hts:", "https://");
				redirect = redirect.Replace("ht:", "http://");
				//Response.Redirect(redirect);
				Result = redirect;
			}
			else
			{
				if (result > 0)
				{
					Result = "<div id='result' style='--hide--'>Registro alterado com sucesso.</div>";
				}
				else
				{
					Result = "<div id='result' style='--hide--'>Nenhum registro foi alterado.</div>";
				}
			}
			return Result;
		}

		private static string ReplaceTemplateLinks(string body, string idTemplate, string idUser)
        {
			string statsUrl = Constants.APP_URL + "/Stats/?";
            Match m;
            string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
            List<string> OldUrls = new List<string>();
            List<string> NewUrls = new List<string>();
            try
            {
                m = Regex.Match(body, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
					string oldUrl = m.Groups[0].ToString();// com 'href='
					string oldUrl_no_href = m.Groups[1].ToString();// sem 'href='
					if (oldUrl.Contains("href"))
					{
						oldUrl_no_href = oldUrl_no_href.Replace("&", "&amp;");
						//oldUrl_no_href = oldUrl_no_href.Replace("\\", "");
						OldUrls.Add(oldUrl);

						string newUrl = "";
						newUrl = statsUrl + "c/" + idTemplate + "/" + idUser + "/" + oldUrl_no_href;
						newUrl = "href='" + newUrl + "'";
						NewUrls.Add(newUrl);
					}
                    m = m.NextMatch();
                }
                for (int i = 0; i < OldUrls.Count(); i++)
                {
                    body = body.Replace(OldUrls[i], NewUrls[i]);
                }
            }
            catch (Exception)
            {
            }
			return body;
        }
	}
}