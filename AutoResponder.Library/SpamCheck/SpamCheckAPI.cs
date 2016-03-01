using System.Collections.Generic;
using AutoResponder.Postmark.Spamcheck;

namespace AutoResponder.Library.SpamCheck
{
	public class SpamcheckResult
	{
		public bool Success { get; set; }
		public string Score { get; set; }
		public string Message { get; set; }
		public string Report { get; set; }
	}

	/// <summary>
	/// http://spamcheck.postmarkapp.com/doc
	/// </summary>
	public class SpamCheckAPI
	{
		public string SpamDetection(string content)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();
			
			string email = buildheaders() + content;
			Api.AddArgument(arguments, "email", email); // The raw dump of the email to be filtered, including all headers.
			Api.AddArgument(arguments, "options", "long"); // Must either be "long" for a full report of processing rules, or "short" for a score request.

			return Api.SendPostRequest("http://spamcheck.postmarkapp.com/filter", arguments, "application/json");
		}

		/// <summary>
		/// https://github.com/krasio/postmark_spamcheck
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public string SpamDetection_ThirdParty(string content)
		{
			string email = buildheaders() + content;
			PostmarkSpamcheck spamChecker = new PostmarkSpamcheck();
			var report = spamChecker.GetReport(email);
			return report.Message;
		}

		private string buildheaders()
		{
			string headers = "";
			return headers;
		}
	}
}