using Mandrill;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AutoResponder.Library
{
	public class Mandril
	{
		static MandrillApi api = new MandrillApi(Constants.SMTP_Password);

		public static string SendMail(String subject, String body, String userName, String email, String IDUser = "", String IDTemplate = "")
		{
			EmailMessage msg = new EmailMessage();

			msg.from_email = Constants.DEFAULT_SENDER_EMAIL;

			List<EmailAddress> list = new List<EmailAddress>();
			list.Add(new EmailAddress { email = email, name = userName });
			msg.to = list;

			msg.subject = subject;
			msg.text = body;

			msg.headers.Add("Return-Path", Constants.RETURN_PATH);
			msg.headers.Add("IDUser", IDUser);
			msg.headers.Add("IDTemplate", IDTemplate);
			
			api.SendMessage(msg);

			return "OK";
		}

        /// <summary>
        /// SendersList (https://mandrillapp.com/api/docs/senders.JSON.html)
        /// </summary>
        /// <returns></returns>
		public static SenderCall SendersList()
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();
			Api.AddArgument(arguments, "key", Constants.SMTP_Password);

			String resultJson = Api.SendPostRequest("https://mandrillapp.com/api/1.0/senders/list.json", arguments, "application/json");
			if (resultJson != "")
			{
				SenderCall resultCall = (SenderCall)JsonConvert.DeserializeObject(resultJson, typeof(SenderCall));
				return resultCall;
			}
			else
			{
				return null;
			}
		}
	}

	public class SenderCall
	{
		public string address { get; set; }
		public string created_at { get; set; }
		public string sent { get; set; }
		public string hard_bounces { get; set; }
		public string soft_bounces { get; set; }
		public string rejects { get; set; }
		public string complaints { get; set; }
		public string unsubs { get; set; }
		public string opens { get; set; }
		public string clicks { get; set; }
		public string unique_opens { get; set; }
		public string unique_clicks { get; set; }
	}
}