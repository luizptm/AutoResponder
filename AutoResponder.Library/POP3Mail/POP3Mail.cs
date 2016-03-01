//------------------------------------------------------------------------------
// File    : MailPop3.cs 
// Version : 1.10
// Date    : 17. November 2002
// Author  : Karavaev Denis,                 Bruno Podetti
// Email   : karavaev_denis@hotmail.com,     Podetti@gmx.net
// Web     : http://wasp.elcat.kg,           -
//
// Special
// This version has only a limitted error handling. Also reading e-Mail and 
// parsing the mailbody may have some faults.
//
// To do
// Extracting mail attachment, and supporting differenet mime types and content
// types.
//------------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Text;

namespace AutoResponder.Library.POP3Mail
{
	class MailMessage
	{
		public string contentType;
		public string contentTransferEncoding;
		public string message;
	};

	class MailHeader
	{
		public int number;
		public int size;
		public string received;
		public string received2;
		public string received3;
		public string from;
		public string to;
		public string subject;
		public string date;
		public string messageID;
		public string mimeVersion;
		public string contentType;
		public string boundary;
		public string X_Priority;
		public string X_MSMail_Priority;
		public string X_Mailer;
		public string importance;
		public string X_MimeOLE;
		public string X_RCPT_TO;
		public string X_UIDL;
		public string status;

		public ArrayList messages;
	};

	/// <summary>
	/// Minimal POP3 Commands:
	/// 
	/// USER name               valid in the AUTHORIZATION state
	/// PASS string
	/// QUIT
	/// 
	/// STAT                    valid in the TRANSACTION state
	/// LIST [msg]
	/// RETR msg
	/// DELE msg
	/// NOOP
	/// RSET
	/// QUIT
	/// 
	/// Optional POP3 Commands:
	/// APOP name digest        valid in the AUTHORIZATION state
	/// TOP msg n               valid in the TRANSACTION state
	/// UIDL [msg]
	/// 
	/// POP3 Replies:
	/// +OK
	/// -ERR
	/// </summary>
	class POP3Mail
	{
		// variables for pop3 class
		public TcpClient tcpClient;
		public NetworkStream netStream;
		public StreamReader streamReader;

		/// <summary>
		/// Connect and authorized user.
		/// </summary>
		/// <param name="pop3host">
		/// Pop3 Mailserver
		/// </param>
		/// <param name="port">
		/// Port of the mailserver, normaly 110
		/// </param>
		/// <param name="user">
		/// a string identifying a mailbox (required), which is of
		/// significance ONLY to the server
		/// </param>
		/// <param name="pwd">
		/// a server/mailbox-specific password (required)
		/// </param>
		/// <returns></returns>
		public string DoConnect(string pop3host, int port, string user, string pwd)
		{
			try
			{
				// create POP3 connection
				tcpClient = new TcpClient(pop3host, port);

				// initialization
				netStream = tcpClient.GetStream();
				streamReader = new StreamReader(tcpClient.GetStream());
				string retVal = streamReader.ReadLine();
				if (!retVal.StartsWith("+OK"))
					return retVal;

				// send login
				retVal = SendCommand("USER " + user + "\r\n");
				// Possible Responses:
				// +OK name is a valid mailbox
				// -ERR never heard of mailbox name
				if (!retVal.StartsWith("+OK"))
					return retVal;

				// send password
				// Possible Responses:
				// +OK maildrop locked and ready
				// -ERR invalid password
				// -ERR unable to lock maildrop
				return SendCommand("PASS " + pwd + "\r\n");
			}
			catch (Exception err)
			{
				return ("-ERR " + err.ToString());
			}
		}

		/// <summary>
		/// Send a command to the pop3 server
		/// </summary>
		/// <param name="command">
		/// command string
		/// </param>
		/// <returns></returns>
		string SendCommand(string command)
		{
			try
			{
				byte[] bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
				netStream.Write(bData, 0, bData.Length);
				return streamReader.ReadLine();
			}
			catch (Exception err)
			{
				return ("-ERR " + err.ToString());
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public string GetCommandBlock(string command)
		{
			StringBuilder CommandBlock = new StringBuilder();
			try
			{
				string sTemp = SendCommand(command);
				if (sTemp.StartsWith("+OK"))
				{
					do
					{
						CommandBlock.Append(sTemp + "\r\n");
						sTemp = streamReader.ReadLine();
					} while (sTemp != ".");
				}
				else
				{
					return sTemp;
				}
			}
			catch (Exception err)
			{
				return ("-ERR " + err.ToString());
			}
			return CommandBlock.ToString();
		}

		/// <summary>
		/// Restrictions:
		///  may only be given in the TRANSACTION state
		///
		/// The POP3 server issues a positive response with a line
		/// containing information for the maildrop.  This line is
		/// called a "drop listing" for that maildrop.
		/// In order to simplify parsing, all POP3 servers are
		/// required to use a certain format for drop listings.  The
		/// positive response consists of "+OK" followed by a single
		/// space, the number of messages in the maildrop, a single
		/// space, and the size of the maildrop in octets.  This memo
		/// makes no requirement on what follows the maildrop size.
		/// Minimal implementations should just end that line of the
		/// response with a CRLF pair.  More advanced implementations
		/// may include other information.
		/// NOTE: This memo STRONGLY discourages implementations
		/// from supplying additional information in the drop
		/// listing.  Other, optional, facilities are discussed
		/// later on which permit the client to parse the messages
		/// in the maildrop.
		/// Note that messages marked as deleted are not counted in
		/// either total.
		/// </summary>
		/// <returns>
		/// Possible Responses:
		/// +OK nn mm
		/// </returns>
		public string GetStat()
		{
			// Send STAT command to get number of mail and total size
			return SendCommand("STAT\r\n");
		}

		public string GetList()// Send LIST command with no parametrs to get all information
		{
			// For saving 'list' results
			return GetCommandBlock("LIST\r\n");
		}

		/// <summary>
		/// Restrictions: 
		///   may only be given in the TRANSACTION state
		/// 
		/// If an argument was given and the POP3 server issues a
		/// positive response with a line containing information for
		/// that message.  This line is called a "scan listing" for
		/// that message.
		/// If no argument was given and the POP3 server issues a
		/// positive response, then the response given is multi-line.
		/// After the initial +OK, for each message in the maildrop,
		/// the POP3 server responds with a line containing
		/// information for that message.  This line is also called a
		/// "scan listing" for that message.  If there are no
		/// messages in the maildrop, then the POP3 server responds
		/// with no scan listings--it issues a positive response
		/// followed by a line containing a termination octet and a
		/// CRLF pair.
		/// In order to simplify parsing, all POP3 servers are
		/// required to use a certain format for scan listings. A
		/// scan listing consists of the message-number of the
		/// message, followed by a single space and the exact size of
		/// the message in octets. This memo makes no requirement on 
		/// what follows the message size in the scan listing. Minimal
		/// implementations should just end that line of the response
		/// with a CRLF pair.  More advanced implementations may
		/// include other information, as parsed from the message.
		/// 
		/// NOTE: This memo STRONGLY discourages implementations
		/// from supplying additional information in the scan
		/// listing.  Other, optional, facilities are discussed
		/// later on which permit the client to parse the messages
		/// in the maildrop.
		/// Note that messages marked as deleted are not listed.
		/// </summary>
		/// <param name="num">
		/// a message-number (optional), which, if present, may NOT 
		/// refer to a message marked as deleted
		/// </param>
		/// <returns>
		/// Possible Responses: 
		/// +OK scan listing follows
		/// -ERR no such message
		/// </returns>
		public string GetList(int num)
		{
			return SendCommand("LIST " + num + "\r\n");
		}

		/// <summary>
		/// Read each part of a multipart message
		/// </summary>
		/// <param name="mailHeader">
		/// </param>
		/// <returns>
		/// </returns>
		public string ReadMessage(MailHeader mailHeader)
		{
			if (mailHeader.messages == null)
			{
				mailHeader.messages = new ArrayList();
			}

			MailMessage mailMessage = new MailMessage();
			string delimiter = "--" + mailHeader.boundary;

			StringBuilder sBody = new StringBuilder();

			string sTemp = streamReader.ReadLine();

			sBody.Append(sTemp);
			bool readHeder = true;
			while (true)
			{
				sTemp = streamReader.ReadLine();
				if (readHeder)
				{
					if (sTemp.Length > 0 && sTemp[0] <= ' ')
					{
						sBody.Append(sTemp);
						continue;
					}

					string key = sBody.ToString();
					sBody.Length = 0;
					if (sTemp.Length == 0)
					{
						readHeder = false;
					}
					else
					{
						sBody.Append(sTemp);
					}

					if (key.StartsWith("Content-Type: "))
					{
						mailMessage.contentType = key.Remove(0, 14);
					}
					else if (key.StartsWith("Content-Transfer-Encoding: "))
					{
						mailMessage.contentTransferEncoding = key.Remove(0, 27);
					}
				}
				else if (sTemp.StartsWith(delimiter))
				{
					mailMessage.message = sBody.ToString();
					mailHeader.messages.Add(mailMessage);

					if (sTemp.Remove(0, delimiter.Length) == "--")
					{
						return sBody.ToString();
					}

					mailMessage = new MailMessage();
					sBody.Length = 0;
					sBody.Append(streamReader.ReadLine());
					readHeder = true;
				}
				else
				{
					sBody.Append(sTemp);
					sBody.Append("\r\n");
				}
			}
		}

		public bool SetMailHeader(MailHeader mailHeader, string key)
		{
			if (key.StartsWith("Received: "))
			{
				if (mailHeader.received == null)
				{
					mailHeader.received = key.Remove(0, 10);
				}
				else if (mailHeader.received2 == null)
				{
					mailHeader.received2 = key.Remove(0, 10);
				}
				else if (mailHeader.received3 == null)
				{
					mailHeader.received3 = key.Remove(0, 10);
				}
			}
			else if (key.StartsWith("From: "))
			{
				mailHeader.from = key.Remove(0, 6);
			}
			else if (key.StartsWith("To: "))
			{
				mailHeader.to = key.Remove(0, 4);
			}
			else if (key.StartsWith("Subject: "))
			{
				mailHeader.subject = key.Remove(0, 9);
			}
			else if (key.StartsWith("Date: "))
			{
				mailHeader.date = key.Remove(0, 6);
			}
			else if (key.StartsWith("Message-ID: "))
			{
				mailHeader.messageID = key.Remove(0, 12);
			}
			else if (key.StartsWith("MIME-Version: "))
			{
				mailHeader.mimeVersion = key.Remove(0, 14);
			}
			else if (key.StartsWith("Content-Type: "))
			{
				int index = key.IndexOf("boundary=");
				mailHeader.boundary = "";

				if (index != -1)
				{
					if (key[index + 9] == '\"')
					{
						int index2 = key.IndexOf("\"", index + 10);
						mailHeader.boundary = key.Substring(index + 10, index2 - index - 10);
					}
					key = key.Substring(0, index);
				}
				mailHeader.contentType = key.Remove(0, 14);
			}
			else if (key.StartsWith("X-Priority: "))
			{
				mailHeader.X_Priority = key.Remove(0, 12);
			}
			else if (key.StartsWith("X-MSMail-Priority: "))
			{
				mailHeader.X_MSMail_Priority = key.Remove(0, 19);
			}
			else if (key.StartsWith("X-Mailer: "))
			{
				mailHeader.X_Mailer = key.Remove(0, 10);
			}
			else if (key.StartsWith("Importance: "))
			{
				mailHeader.importance = key.Remove(0, 12);
			}
			else if (key.StartsWith("X-MimeOLE: "))
			{
				mailHeader.X_MimeOLE = key.Remove(0, 11);
			}
			else if (key.StartsWith("X-RCPT-TO: "))
			{
				mailHeader.X_RCPT_TO = key.Remove(0, 11);
			}
			else if (key.StartsWith("X-UIDL: "))
			{
				mailHeader.X_UIDL = key.Remove(0, 8);
			}
			else if (key.StartsWith("Status: "))
			{
				mailHeader.status = key.Remove(0, 8);
			}
			else
				return false;

			return true;
		}

		/// <summary>
		/// Get all mails form the pop3 server.
		/// </summary>
		/// <param name="body">
		/// If set it returns mailheader and message.
		/// </param>
		/// <returns></returns>
		public MailHeader[] GetMails(bool body)
		{
			string list = GetList();
			if (list == null || list.Length == 0 || !list.StartsWith("+OK"))
				return null;

			string[] param = list.Split('\n');
			string[] listHeader = param[0].Split(' ');

			int count = int.Parse(listHeader[1]);

			MailHeader[] myHeader = new MailHeader[count];
			for (int n = 0; n < count; n++)
			{
				string[] msg = param[n + 1].Split(' ');

				myHeader[n] = GetMail(int.Parse(msg[0]), body);
			}
			return myHeader;
		}

		public MailHeader GetMail(int num, bool body)
		{
			MailHeader mailHeader = null;
			try
			{
				string sTemp;
				if (body == true)
					sTemp = SendCommand("RETR " + num + "\r\n");
				else
					sTemp = SendCommand("TOP " + num + "\r\n");

				if (sTemp.StartsWith("+OK"))
				{
					string[] param = sTemp.Split(' ');

					string delimiter = "--";
					mailHeader = new MailHeader();
					mailHeader.number = num;
					mailHeader.size = int.Parse(param[1]);

					StringBuilder sBody = new StringBuilder();

					while (true)
					{
						sTemp = streamReader.ReadLine();
						if (sTemp == ".")
						{
							SetMailHeader(mailHeader, sBody.ToString());
							break;
						}
						else if (sTemp.Length > 0 && sTemp[0] <= ' ')
						{
							sBody.Append(sTemp);
						}
						// Begin off message body
						else if (sTemp.StartsWith(delimiter))
						{
							// new Message
							ReadMessage(mailHeader);
						}
						else
						{
							if (sBody.Length > 0)
							{
								SetMailHeader(mailHeader, sBody.ToString());
								delimiter = "--" + mailHeader.boundary;
							}

							sBody = new StringBuilder(sTemp);
						}
					}
					// . - is the end of the server response
				}
			}
			catch (Exception)
			{
				return null;
			}
			return mailHeader;
		}

		/// <summary>
		/// If the POP3 server issues a positive response, then the
		/// response given is multi-line.  After the initial +OK, the
		/// POP3 server sends the message corresponding to the given
		/// message-number, being careful to byte-stuff the termination
		/// character (as with all multi-line responses).
		/// </summary>
		/// <param name="num">
		/// a message-number (required) which may NOT refer to a
		/// message marked as deleted
		/// </param> 
		/// <returns>
		/// Possible Responses:
		/// +OK message follows
		/// -ERR no such message
		/// </returns>
		public string Retr(int num)
		{
			return GetCommandBlock("RETR " + num + "\r\n");
		}

		/// <summary>
		/// The POP3 server marks the message as deleted.  Any future
		/// reference to the message-number associated with the message
		/// in a POP3 command generates an error.  The POP3 server does
		/// not actually delete the message until the POP3 session
		/// enters the UPDATE state.
		/// </summary>
		/// <param name="num">
		/// a message-number (required) which may NOT refer to a 
		/// message marked as deleted
		/// </param>
		/// <returns>
		/// Possible Responses:
		/// +OK message deleted
		/// -ERR no such message
		/// </returns>
		public string Dele(int num)
		{
			return SendCommand("DELE " + num + "\r\n");
		}

		/// <summary>
		/// The POP3 server does nothing, it merely replies with a
		/// positive response.
		/// </summary>
		/// <returns>
		/// Possible Responses:
		/// +OK
		/// </returns>
		public string GetNoop()
		{
			return SendCommand("NOOP\r\n");
		}

		/// <summary>
		/// If any messages have been marked as deleted by the POP3
		/// server, they are unmarked.  The POP3 server then replies
		/// Send RSET command to unmark all deleteting messages
		/// </summary>
		/// <returns>
		/// Possible Responses:
		/// +OK
		/// </returns>
		public string Rset()
		{
			return SendCommand("RSET\r\n");
		}

		/// <summary>
		/// The POP3 server removes all messages marked as deleted
		/// from the maildrop and replies as to the status of this
		/// operation.  If there is an error, such as a resource
		/// shortage, encountered while removing messages, the
		/// maildrop may result in having some or none of the messages
		/// marked as deleted be removed.  In no case may the server
		/// remove any messages not marked as deleted.
		/// Whether the removal was successful or not, the server
		/// then releases any exclusive-access lock on the maildrop
		/// and closes the TCP connection.
		/// </summary>
		/// <returns>
		/// Possible Responses:
		/// +OK
		/// -ERR some deleted messages not removed
		/// </returns>
		public string Quit()
		{
			if (netStream == null || streamReader == null)
			{
				return ("-ERR no connection");
			}
			string tmp = SendCommand("QUIT\r\n"); ;
			netStream.Close();
			streamReader.Close();
			return tmp;
		}

		/// <summary>
		/// If the POP3 server issues a positive response, then the
		/// response given is multi-line.  After the initial +OK, the
		/// POP3 server sends the headers of the message, the blank
		/// line separating the headers from the body, and then the
		/// number of lines of the indicated message's body, being
		/// careful to byte-stuff the termination character (as with
		/// all multi-line responses).
		/// Note that if the number of lines requested by the POP3
		/// client is greater than than the number of lines in the
		/// body, then the POP3 server sends the entire message.
		/// </summary>
		/// <param name="num_mess">
		/// a message-number (required) which may NOT refer to to a 
		/// message marked as deleted, and
		/// </param>
		/// <param name="num_strok">
		///  a non-negative number of lines (required)
		/// </param>
		/// <returns>
		/// Possible Responses: 
		/// +OK top of message follows
		/// -ERR no such message
		/// </returns>
		public string GetTop(int num_mess, int num_strok)
		{
			return GetCommandBlock("TOP " + num_mess + " " + num_strok + "\r\n");
		}

		public string GetTop(int num)
		{
			return GetCommandBlock("TOP " + num + "\r\n");
		}

		/// <summary>
		/// If an argument was given and the POP3 server issues a positive 
		/// response with a line containing information for that message.
		/// This line is called a "unique-id listing" for that message.
		/// If no argument was given and the POP3 server issues a positive
		/// response, then the response given is multi-line.  After the
		/// initial +OK, for each message in the maildrop, the POP3 server
		/// responds with a line containing information for that message.
		/// This line is called a "unique-id listing" for that message.
		/// In order to simplify parsing, all POP3 servers are required to
		/// use a certain format for unique-id listings.  A unique-id
		/// listing consists of the message-number of the message,
		/// followed by a single space and the unique-id of the message.
		/// No information follows the unique-id in the unique-id listing. 
		/// The unique-id of a message is an arbitrary server-determined
		/// string, consisting of one to 70 characters in the range 0x21
		/// to 0x7E, which uniquely identifies a message within a
		/// maildrop and which persists across sessions.  This
		/// persistence is required even if a session ends without
		/// entering the UPDATE state.  The server should never reuse an
		/// unique-id in a given maildrop, for as long as the entity
		/// using the unique-id exists.
		/// Note that messages marked as deleted are not listed.
		/// While it is generally preferable for server implementations
		/// to store arbitrarily assigned unique-ids in the maildrop,
		/// this specification is intended to permit unique-ids to be
		/// calculated as a hash of the message.  Clients should be able
		/// to handle a situation where two identical copies of a
		/// message in a maildrop have the same unique-id.
		/// </summary>
		/// <param name="num">
		/// a message-number (optional), which, if present, may NOT
		/// refer to a message marked as deleted
		/// </param>
		/// <returns>
		/// Possible Responses:
		/// +OK unique-id listing follows
		/// -ERR no such message
		/// </returns>
		public string GetUidl(int num)
		{
			return SendCommand("UIDL " + num + "\r\n");
		}

		public string GetUidl()
		{
			return GetCommandBlock("UIDL\r\n");
		}
	}
}
