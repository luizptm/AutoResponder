using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace AutoResponder.Library.POP3
{
public class Util
	{
		protected static Regex BoundaryRegex =
		  new Regex("Content-Type: multipart(?:/\\S+;)" +
		  "\\s+[^\r\n]*boundary=\"?(?<boundary>" +
		  "[^\"\r\n]+)\"?\r\n", RegexOptions.IgnoreCase |
		  RegexOptions.Compiled);
		protected static Regex UtcDateTimeRegex = new Regex(
		  @"^(?:\w+,\s+)?(?<day>\d+)\s+(?<month>\w+)\s+(?<year>\d+)\s+(?<hour>\d{1,2})" +
		  @":(?<minute>\d{1,2}):(?<second>\d{1,2})\s+(?<offsetsign>\-|\+)(?<offsethours>" +
		  @"\d{2,2})(?<offsetminutes>\d{2,2})(?:.*)$",
		  RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static NameValueCollection ParseHeaders(string headerText)
		{
			NameValueCollection headers = new NameValueCollection();
			StringReader reader = new StringReader(headerText);

			string line;
			string headerName = null, headerValue;
			int colonIndx;

			while ((line = reader.ReadLine()) != null)
			{
				if (line == "")
					break;

				if (Char.IsLetterOrDigit(line[0]) && (colonIndx = line.IndexOf(':')) != -1)
				{
					headerName = line.Substring(0, colonIndx);
					headerValue = line.Substring(colonIndx + 1).Trim();

					headers.Add(headerName, headerValue);
				}
				else if (headerName != null)
					headers[headerName] += " " + line.Trim();
				else
					throw new FormatException("Could not parse headers");
			}

			return headers;
		}

		public static List<MessagePart> ParseMessageParts(string emailText)
		{
			List<MessagePart> messageParts = new List<MessagePart>();
			int newLinesIndx = emailText.IndexOf("\r\n\r\n");

			Match m = BoundaryRegex.Match(emailText);

			if (m.Index < emailText.IndexOf("\r\n\r\n") && m.Success)
			{
				string boundary = m.Groups["boundary"].Value;
				string startingBoundary = "\r\n--" + boundary;

				int startingBoundaryIndx = -1;

				while (true)
				{
					if (startingBoundaryIndx == -1)
						startingBoundaryIndx = emailText.IndexOf(startingBoundary);

					if (startingBoundaryIndx != -1)
					{
						int nextBoundaryIndx = emailText.IndexOf(startingBoundary,
						  startingBoundaryIndx + startingBoundary.Length);

						if (nextBoundaryIndx != -1 && nextBoundaryIndx != startingBoundaryIndx)
						{
							string multipartMsg = emailText.Substring(startingBoundaryIndx +
							  startingBoundary.Length,
							  (nextBoundaryIndx - startingBoundaryIndx - startingBoundary.Length));

							int headersIndx = multipartMsg.IndexOf("\r\n\r\n");

							if (headersIndx == -1)
								throw new FormatException("Incompatible multipart message format");

							string bodyText = multipartMsg.Substring(headersIndx).Trim();

							NameValueCollection headers = Util.ParseHeaders(multipartMsg.Trim());
							messageParts.Add(new MessagePart(headers, bodyText));
						}
						else
							break;

						startingBoundaryIndx = nextBoundaryIndx;
					}
					else
						break;
				}

				if (newLinesIndx != -1)
				{
					string emailBodyText = emailText.Substring(newLinesIndx + 1);
				}
			}
			else
			{
				int headersIndx = emailText.IndexOf("\r\n\r\n");

				if (headersIndx == -1)
					throw new FormatException("Incompatible multipart message format");

				string bodyText = emailText.Substring(headersIndx).Trim();

				NameValueCollection headers = Util.ParseHeaders(emailText);
				messageParts.Add(new MessagePart(headers, bodyText));
			}

			return messageParts;
		}
		public static DateTime ConvertStrToUtcDateTime(string str)
		{
			Match m = UtcDateTimeRegex.Match(str);

			int day, month, year, hour, min, sec;

			if (m.Success)
			{
				day = Convert.ToInt32(m.Groups["day"].Value);
				year = Convert.ToInt32(m.Groups["year"].Value);
				hour = Convert.ToInt32(m.Groups["hour"].Value);
				min = Convert.ToInt32(m.Groups["minute"].Value);
				sec = Convert.ToInt32(m.Groups["second"].Value);

				switch (m.Groups["month"].Value)
				{
					case "Jan":
						month = 1;
						break;
					case "Feb":
						month = 2;
						break;
					case "Mar":
						month = 3;
						break;
					case "Apr":
						month = 4;
						break;
					case "May":
						month = 5;
						break;
					case "Jun":
						month = 6;
						break;
					case "Jul":
						month = 7;
						break;
					case "Aug":
						month = 8;
						break;
					case "Sep":
						month = 9;
						break;
					case "Oct":
						month = 10;
						break;
					case "Nov":
						month = 11;
						break;
					case "Dec":
						month = 12;
						break;
					default:
						throw new FormatException("Unknown month.");
				}

				string offsetSign = m.Groups["offsetsign"].Value;
				int offsetHours = Convert.ToInt32(m.Groups["offsethours"].Value);
				int offsetMinutes = Convert.ToInt32(m.Groups["offsetminutes"].Value);

				DateTime dt = new DateTime(year, month, day, hour, min, sec);

				if (offsetSign == "+")
				{
					dt.AddHours(offsetHours);
					dt.AddMinutes(offsetMinutes);
				}
				else if (offsetSign == "-")
				{
					dt.AddHours(-offsetHours);
					dt.AddMinutes(-offsetMinutes);
				}

				return dt;
			}

			throw new FormatException("Incompatible date/time string format");
		}
	}
}