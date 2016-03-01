using System;

namespace AutoResponder.Postmark.Spamcheck
{
	public class SpamcheckScoreRequest
	{
		public string email { get; set; }

		public string options { get; set; }
	}
}

