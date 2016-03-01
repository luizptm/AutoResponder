using System;

namespace AutoResponder.Postmark.Spamcheck
{
	public interface IHttpClient
	{
		IHttpResponse Post(string uri, string postBody, string accept = null, string contentType = null);
	}
}

