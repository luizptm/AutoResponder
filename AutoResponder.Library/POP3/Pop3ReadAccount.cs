using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AutoResponder.Library.POP3;
using AutoResponder.Models.Entity;

namespace AutoResponder.Library
{
	public class Pop3ReadAccount
	{
		public static String LerBounces()
		{
			int totalBounces = 0;

            Email email = null;
            List<MessagePart> msgParts = null;
			//try
			//{
				int popPort = Int32.Parse(Constants.POP_Port);
				using (Pop3Client client = new Pop3Client(Constants.POP_Host, popPort, Constants.POP_Email, Constants.POP_Password, true))
				{
					string err = client.Connect();

					if (!err.StartsWith("+OK"))
					{
						client.Close();
						return err;
					}

					int totalEmails = client.GetEmailCount();
					List<Email> emails;
					emails = client.FetchEmailList(1, totalEmails);
					foreach (var b in emails)
					{
						int emailId = totalEmails--;
						email = client.FetchEmail(emailId); // Fetching email headers
						msgParts = client.FetchMessageParts(emailId); // Fetching email body

						string body = "";
						string contentType = "";
						string contentTransferEncoding = "";
						string charset = "";
						MessagePart preferredMsgPart = Pop3Mail.FindMessagePart(msgParts, "text/html");
						if (preferredMsgPart == null)
						{
							preferredMsgPart = Pop3Mail.FindMessagePart(msgParts, "text/plain");
						}
						if (preferredMsgPart != null)
						{
							if (preferredMsgPart.Headers["Content-Type"] != null)
							{
								contentType = preferredMsgPart.Headers["Content-Type"];
							}
							charset = "us-ascii"; // default charset
							if (preferredMsgPart.Headers["Content-Transfer-Encoding"] != null)
							{
								contentTransferEncoding = preferredMsgPart.Headers["Content-Transfer-Encoding"];
							}

							Match m = Pop3Mail.CharsetRegex.Match(contentType);
							if (m.Success)
							{
								charset = m.Groups["charset"].Value;
							}

							if (contentTransferEncoding != "")
							{
								if (contentTransferEncoding.ToLower() == "base64")
									body = Pop3Mail.DecodeBase64String(charset, preferredMsgPart.MessageText);
								else if (contentTransferEncoding.ToLower() == "quoted-printable")
									body = Pop3Mail.DecodeQuotedPrintableString(preferredMsgPart.MessageText);
								else
									body = preferredMsgPart.MessageText;
							}
							else
							{
								body = preferredMsgPart.MessageText;
							}
						}

						String content = preferredMsgPart != null ? (preferredMsgPart.Headers["Content-Type"].IndexOf("text/plain") != -1 ? "<pre>" + FormatUrls(body) + "</pre>" : body) : null;

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
							BounceWeight = Pop3Mail.BounceSeverity(content);
						}
						
						if (IDUser != 0 && IDTemplate != 0)
						{
							int bounce = SalvaStat(IDUser, IDTemplate, BounceWeight);
							totalBounces = totalBounces + bounce;
						}
					}
				}
			//}
			//catch { }

			return totalBounces.ToString();
		}

		private static string FormatUrls(string plainText)
        {
            string replacementLink = "<a href=\"${url}\">${url}</a>";

            return Pop3Mail.UrlRegex.Replace(plainText, replacementLink);
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