using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AutoResponder.Library.POP3;

namespace AutoResponder.Library.POP3
{
	public class Email
	{
		public NameValueCollection Headers { get; protected set; }

		public string ContentType { get; protected set; }
		public DateTime UtcDateTime { get; protected set; }
		public string From { get; protected set; }
		public string To { get; protected set; }
		public string Subject { get; protected set; }
		public string Body { get; protected set; }

		public Email(string emailText)
		{
			Headers = Util.ParseHeaders(emailText);

			ContentType = Headers["Content-Type"];
			From = Headers["From"];
			To = Headers["To"];
			Subject = Headers["Subject"];
			Body = Headers["Body"];

			if (Headers["Date"] != null)
			{
				try
				{
					UtcDateTime = Util.ConvertStrToUtcDateTime(Headers["Date"]);
				}
				catch (FormatException)
				{
					UtcDateTime = DateTime.MinValue;
				}
			}
			else
			{
				UtcDateTime = DateTime.MinValue;
			}
		}
	}
}