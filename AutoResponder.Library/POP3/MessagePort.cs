using System.Collections.Specialized;

namespace AutoResponder.Library.POP3
{
	public class MessagePart
	{
		public NameValueCollection Headers { get; protected set; }

		public string ContentType { get; protected set; }
		public string MessageText { get; protected set; }

		public MessagePart(NameValueCollection headers, string messageText)
		{
			Headers = headers;
			ContentType = Headers["Content-Type"];
			MessageText = messageText;
		}
	}
}