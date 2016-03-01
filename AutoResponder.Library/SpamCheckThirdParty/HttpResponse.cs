using System.IO;
using System.Net;

namespace AutoResponder.Postmark.Spamcheck
{
	internal class HttpResponse : IHttpResponse
	{
		private readonly Stream response;
		
		public HttpResponse(WebResponse response)
		{
			this.response = response.GetResponseStream();	
		}
		
		public string Body
		{
			get 
			{
				using (var sr = new StreamReader(this.response))
				{
					return sr.ReadToEnd();	
				}
			}
		}
	}
}

