using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoResponder.Library.POP3
{
	public class Pop3Mail
	{
		public static Regex CharsetRegex =
			new Regex("charset=\"?(?<charset>[^\\s\"]+)\"?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static Regex QuotedPrintableRegex =
			new Regex("=(?<hexchars>[0-9a-fA-F]{2,2})",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static Regex UrlRegex =
			new Regex("(?<url>https?://[^\\s\"]+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static Regex FilenameRegex =
			new Regex("filename=\"?(?<filename>[^\\s\"]+)\"?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static Regex NameRegex =
			new Regex("name=\"?(?<filename>[^\\s\"]+)\"?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static void CountBounce(int emailId)
		{
			/* Creating Pop3Client object and accessing email data */
			Email email = null;
			List<MessagePart> msgParts = null;

			int popPort = Int32.Parse(Constants.POP_Port);
			using (Pop3Client client = new Pop3Client(Constants.POP_Host, popPort, Constants.POP_Email, Constants.POP_Password, true))
			{
				client.Connect();

				// Fetching email headers
				email = client.FetchEmail(emailId);
				// Fetching email body
				msgParts = client.FetchMessageParts(emailId);
			}

			/* Selecting a message part to display to the user */
			MessagePart preferredMsgPart = FindMessagePart(msgParts, "text/html");

			if (preferredMsgPart == null)
				preferredMsgPart = FindMessagePart(msgParts, "text/plain");
			if (preferredMsgPart == null && msgParts.Count > 0)
				preferredMsgPart = msgParts[0];

			string contentType, charset, contentTransferEncoding, body = null;

			if (preferredMsgPart != null)
			{
				contentType = preferredMsgPart.Headers["Content-Type"];
				charset = "us-ascii"; // default charset
				contentTransferEncoding =
					preferredMsgPart.Headers["Content-Transfer-Encoding"];

				Match m = CharsetRegex.Match(contentType);
				if (m.Success)
				{
					charset = m.Groups["charset"].Value;
				}

				/* Decoding base64 and quoted-printable encoded messages */
				if (contentTransferEncoding != null)
				{
					if (contentTransferEncoding.ToLower() == "base64")
					{
						body = DecodeBase64String(charset, preferredMsgPart.MessageText);
					}
					else if (contentTransferEncoding.ToLower() == "quoted-printable")
					{
						body = DecodeQuotedPrintableString(preferredMsgPart.MessageText);
					}
					else
					{
						body = preferredMsgPart.MessageText;
					}
				}
				else
					body = preferredMsgPart.MessageText;
			}

			String xfailed = email.Headers["X-Failed-Recipients"];
		}

		public static int BounceSeverity(string texto)
		{
			string[] words = texto.Split(' ');
			List<string> palavs = new List<string>();

			foreach (var word in words)
			{
				if (!palavs.Contains(word) || palavs.Count() < 1)
					palavs.Add(word);
			}
			int weight = 0;

			if (texto.Contains("4.4.5"))
				weight += 1;
			if (texto.Contains("4.4.7"))
				weight += 1;
			if (texto.Contains("4.4.8"))
				weight += 2;
			if (texto.Contains("5.1.1"))
				weight += 2;
			if (texto.Contains("5.1.2"))
				weight += 2;
			if (texto.Contains("5.2.1"))
				weight += 2;
			if (texto.Contains("5.3.0"))
				weight += 1;
			if (texto.Contains("5.4.0"))
				weight += 1;
			if (texto.Contains("5.5.0"))
				weight += 2;
			if (texto.Contains("5.5.6"))
				weight += 2;
			if (texto.Contains("5.7.0"))
				weight += 1;
			if (texto.Contains("5.7.1"))
				weight += 1;
			if (texto.Contains("5.7.2"))
				weight += 2;
			if (texto.Contains("5.0.0 Service unavailable"))
				weight += 2;
			if (texto.Contains("5.x.0 - Message bounced by administrator"))
				weight += 2;

			return weight;
		}

		public static List<string> extracEmails(string text)
		{
			string MatchEmailPattern =
				@"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
				+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
				+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
				+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
			Regex rx = new Regex(MatchEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			// Find matches.
			MatchCollection matches = rx.Matches(text);
			// Report the number of matches found.

			int noOfMatches = matches.Count;
			// Report on each match.
			List<string> emails = new List<string>();
			foreach (Match match in matches)
			{
				emails.Add(match.Value.ToString());
			}
			return emails;
		}

		public static Decoder GetDecoder(string charset)
		{
			Decoder decoder;

			switch (charset.ToLower())
			{
				case "utf-7":
					decoder = Encoding.UTF7.GetDecoder();
					break;
				case "utf-8":
					decoder = Encoding.UTF8.GetDecoder();
					break;
				case "us-ascii":
					decoder = Encoding.ASCII.GetDecoder();
					break;
				case "iso-8859-1":
					decoder = Encoding.ASCII.GetDecoder();
					break;
				default:
					decoder = Encoding.ASCII.GetDecoder();
					break;
			}

			return decoder;
		}

		public static string DecodeBase64String(string charset, string encodedString)
		{
			Decoder decoder = GetDecoder(charset);

			byte[] buffer = Convert.FromBase64String(encodedString);
			char[] chararr = new char[decoder.GetCharCount(buffer, 0, buffer.Length)];

			decoder.GetChars(buffer, 0, buffer.Length, chararr, 0);

			return new string(chararr);
		}

		public static string DecodeQuotedPrintableString(string encodedString)
		{
			StringBuilder b = new StringBuilder();
			int startIndex = 0;

			MatchCollection matches = QuotedPrintableRegex.Matches(encodedString);

			for (int i = 0; i < matches.Count; i++)
			{
				Match m = matches[i];
				string hexchars = m.Groups["hexchars"].Value;
				int charcode = Convert.ToInt32(hexchars, 16);
				char c = (char)charcode;

				if (m.Index > 0)
					b.Append(encodedString.Substring(startIndex,
						(m.Index - startIndex)));

				b.Append(c);

				startIndex = m.Index + 3;
			}

			if (startIndex < encodedString.Length)
				b.Append(encodedString.Substring(startIndex));

			return Regex.Replace(b.ToString(), "=\r\n", "");
		}

		public static MessagePart FindMessagePart(List<MessagePart> msgParts, string contentType)
		{
			foreach (MessagePart p in msgParts)
			{
				if (p.ContentType != null && p.ContentType.IndexOf(contentType) != -1)
				{
					return p;
				}
			}
			return null;
		}
	}
}