using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using AutoResponder.Library.POP3;

namespace AutoResponder.Library.POP3
{
	public class Pop3Client : IDisposable
	{
		public string Host { get; protected set; }
		public int Port { get; protected set; }
		public string Email { get; protected set; }
		public string Password { get; protected set; }
		public bool IsSecure { get; protected set; }

		public TcpClient Client { get; protected set; }
		public Stream ClientStream { get; protected set; }

		public StreamWriter Writer { get; protected set; }
		public StreamReader Reader { get; protected set; }

		private bool disposed = false;

		public Pop3Client(string host, int port, string email, string password)
			: this(host, port, email, password, false)
		{
		}

		public Pop3Client(string host, int port, string email,
		  string password, bool secure)
		{
			Host = host;
			Port = port;
			Email = email;
			Password = password;
			IsSecure = secure;
		}

		public string Connect()
		{
			try
			{
				if (Client == null)
					Client = new TcpClient();

				if (!Client.Connected)
					Client.Connect(Host, Port);

				if (IsSecure)
				{
					SslStream secureStream =
					  new SslStream(Client.GetStream());
					secureStream.AuthenticateAsClient(Host);

					ClientStream = secureStream;
					secureStream = null;
				}
				else
					ClientStream = Client.GetStream();

				Writer = new StreamWriter(ClientStream);
				Reader = new StreamReader(ClientStream);

				ReadLine();
				Login();

				return "+OK";
			}
			catch (Exception err)
			{
				return ("-ERR " + err.ToString());
			}
		}

		public int GetEmailCount()
		{
			int count = 0;
			string response = SendCommand("STAT");

			if (IsResponseOk(response))
			{
				string[] arr = response.Substring(4).Split(' ');
				count = Convert.ToInt32(arr[0]);
			}
			else
			{
				count = -1;
			}
			return count;
		}

		public Email FetchEmail(int emailId)
		{
			if (IsResponseOk(SendCommand("TOP " + emailId + " 0")))
				return new Email(ReadLines());
			else
				return null;
		}

		public List<Email> FetchEmailList(int start, int count)
		{
			List<Email> emails = new List<Email>(count);

			for (int i = start; i < (start + count); i++)
			{
				Email email = FetchEmail(i);

				if (email != null)
					emails.Add(email);
			}

			return emails;
		}

		public List<MessagePart> FetchMessageParts(int emailId)
		{
			if (IsResponseOk(SendCommand("RETR " + emailId)))
				return Util.ParseMessageParts(ReadLines());

			return null;
		}

		public void Close()
		{
			if (Client != null)
			{
				if (Client.Connected)
					Logout();

				Client.Close();
				Client = null;
			}

			if (ClientStream != null)
			{
				ClientStream.Close();
				ClientStream = null;
			}

			if (Writer != null)
			{
				Writer.Close();
				Writer = null;
			}

			if (Reader != null)
			{
				Reader.Close();
				Reader = null;
			}

			disposed = true;
		}

		public void DeleteEmail(int emailId)
		{
			if (!IsResponseOk(SendCommand("DELE " + emailId)))
			{
				throw new Exception("User/password not accepted");
			}
			SendCommand("QUIT");
			Dispose();
		}

		public void Dispose()
		{
			if (!disposed)
				Close();
		}

		protected void Login()
		{
			if (!IsResponseOk(SendCommand("USER " + Email)) ||
			  !IsResponseOk(SendCommand("PASS " + Password)))
				throw new Exception("User/password not accepted");
		}

		protected void Logout()
		{
			SendCommand("RSET");
		}

		protected string SendCommand(string cmdtext)
		{
			Writer.WriteLine(cmdtext);
			Writer.Flush();

			return ReadLine();
		}

		protected string ReadLine()
		{
			return Reader.ReadLine() + "\r\n";
		}

		protected string ReadLines()
		{
			StringBuilder b = new StringBuilder();
			while (true)
			{
				string temp = ReadLine();
				if (temp == ".\r\n" || temp.IndexOf("-ERR") != -1)
					break;
				b.Append(temp);
			}
			return b.ToString();
		}

		protected static bool IsResponseOk(string response)
		{
			if (response.StartsWith("+OK"))
				return true;
			if (response.StartsWith("-ERR"))
				return false;

			throw new Exception("Cannot understand server response: " + response);
		}
	}


}