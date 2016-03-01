using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AutoResponder.Library.POP3
{
	public class Pop3SendMail
	{
		public static string SendMail(String subject, String body, String userName, String email, String IDUser = "", String IDTemplate = "")
		{
			SmtpClient client = new SmtpClient(Constants.SMTP_Host);
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(Constants.SMTP_Email, Constants.SMTP_Password);
			client.DeliveryFormat = SmtpDeliveryFormat.International;
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.EnableSsl = true;
			client.Host = Constants.SMTP_Host;
            client.Port = Int32.Parse(Constants.SMTP_Port);
			
			MailMessage msg = new MailMessage();
			msg.From = new System.Net.Mail.MailAddress(Constants.DEFAULT_SENDER_EMAIL);
			msg.Sender = new System.Net.Mail.MailAddress(Constants.DEFAULT_SENDER_EMAIL);
			msg.To.Add(new System.Net.Mail.MailAddress(email, userName));
			//msg.CC.Add(new System.Net.Mail.MailAddress("pmartins@proteste.org.br", "Luiz Paulo Toniazzo"));//DEBUG
			msg.Subject = subject;
			msg.Body = body;
			msg.IsBodyHtml = true;
			msg.Headers.Add("Return-Path", Constants.RETURN_PATH);
			msg.Headers.Add("IDUser", IDUser);
			msg.Headers.Add("IDTemplate", IDTemplate);
			client.Send(msg);

			return "OK";
		}
	}
}