using System;

namespace AutoResponder.Postmark.Spamcheck
{
	public interface IPostmarkSpamcheckWebClient
	{
		string GetSpamcheckResult(SpamcheckScoreRequest request);
	}
}

