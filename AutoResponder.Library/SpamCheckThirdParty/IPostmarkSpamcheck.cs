using System;

namespace AutoResponder.Postmark.Spamcheck
{
	public interface IPostmarkSpamcheck
	{
		SpamcheckResult GetScore(string email);
		SpamcheckResult GetReport(string email);
	}
}

