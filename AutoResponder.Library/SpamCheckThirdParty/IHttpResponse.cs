using System;

namespace AutoResponder.Postmark.Spamcheck
{
	public interface IHttpResponse
	{
		string Body { get; }
	}
}

