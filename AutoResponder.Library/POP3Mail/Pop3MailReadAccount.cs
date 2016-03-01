using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using AutoResponder.Models.Entity;

namespace AutoResponder.Library.POP3Mail
{
	public class Pop3MailReadAccount
	{
		public static string server = Constants.POP_Host;
		public static int port = Int32.Parse(Constants.POP_Port);
		public static string username = Constants.POP_Email;
		public static string password = Constants.POP_Password;

		public static String LerBounces()
		{
			int bouncedMail = 0;
			POP3Mail pop3Connection = new POP3Mail();

			string err = pop3Connection.DoConnect(server, port, username, password);

			if (!err.StartsWith("+OK"))
			{
				pop3Connection.Quit();
				return err;
			}

			MailHeader[] myHeader = pop3Connection.GetMails(false);
			if (myHeader != null)
			{
				for (int n = 0; n < myHeader.Length; n++)
				{
					if (myHeader[n] != null)
					{
						string from = myHeader[n].from;
						string to = myHeader[n].to;
						string date = myHeader[n].date;
						string subject = myHeader[n].subject;
						string contentType = myHeader[n].contentType;
						string content = pop3Connection.ReadMessage(myHeader[n]);

						int IDUser = 0;
						int IDTemplate = 0;
						int BounceWeight = 0;
						if (!String.IsNullOrEmpty(content))
						{
							int padding = 0;
							if (content.Contains("IDUser"))
							{
								string iduser = content.IndexOf("IDUser:").ToString();
								padding = int.Parse(iduser);
								iduser = content.Substring(padding + 7);
								int break_line = iduser.IndexOf("\r\n");
								iduser = iduser.Substring(0, break_line);
								IDUser = int.Parse(iduser);
							}
							if (content.Contains("IDTemplate"))
							{
								string idTemplate = content.IndexOf("IDTemplate:").ToString();
								padding = int.Parse(idTemplate);
								idTemplate = content.Substring(padding + 11);
								int break_line = idTemplate.IndexOf("\r\n");
								idTemplate = idTemplate.Substring(0, break_line);
								IDTemplate = int.Parse(idTemplate);
							}
						}

						if (IDUser != 0 && IDTemplate != 0)
						{
							int bounce = SalvaStat(IDUser, IDTemplate, BounceWeight);
							bouncedMail = bouncedMail + bounce;
						}
					}
				}
			}
			pop3Connection.Quit();

			return bouncedMail.ToString();
		}

		private static int SalvaStat(int IDUser, int IDTemplate, int BounceWeight)
		{
			Entities db = new Entities();
			BR_AutoResponder_Sending reg = db.BR_AutoResponder_Sending.Where(x => x.UserId == IDUser && x.TemplateId == IDTemplate).FirstOrDefault();
			if (reg != null)
			{
				reg.Bounce = true;
				db.Entry(reg).State = EntityState.Modified;
				return db.SaveChanges();
			}

			return 0;
		}
	}
}