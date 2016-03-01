using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AutoResponder.Library
{
	public class Constants
	{
		public static string CACHE_ITEM_KEY = "SchedulerObject";

		#region GLOBAL Constants

		public static int INTERVAL
		{
			get
			{
				if (ConfigurationManager.AppSettings["INTERVAL"] != null)
				{
					string interval = ConfigurationManager.AppSettings["INTERVAL"].ToString();
					try
					{
						return Int32.Parse(interval);
					}
					catch
					{
						return 1;
					}
				}
				return 0;
			}
		}

		public static string APP_URL
		{
			get
			{
				if (ConfigurationManager.AppSettings["APP_URL"] != null)
				{
					return ConfigurationManager.AppSettings["APP_URL"].ToString();
				}
				return "";
			}
		}

		public static string DEFAULT_SENDER_EMAIL
		{
			get
			{
				if (ConfigurationManager.AppSettings["DEFAULT_SENDER_EMAIL"] != null)
				{
					return ConfigurationManager.AppSettings["DEFAULT_SENDER_EMAIL"].ToString();
				}
				return "";
			}
		}

		public static string RETURN_PATH
		{
			get
			{
				if (ConfigurationManager.AppSettings["RETURN-PATH"] != null)
				{
					return ConfigurationManager.AppSettings["RETURN-PATH"].ToString();
				}
				return "";
			}
		}

		public static string USE_POP3
		{
			get
			{
				if (ConfigurationManager.AppSettings["USE_POP3"] != null)
				{
					return ConfigurationManager.AppSettings["USE_POP3"].ToString();
				}
				return "";
			}
		}

		#endregion

		#region POP Constants

		public static string POP_Host
		{
			get
			{
				if (ConfigurationManager.AppSettings["POP_ACCOUNT_HOST"] != null)
				{
					return ConfigurationManager.AppSettings["POP_ACCOUNT_HOST"].ToString();
				}
				return "";
			}
		}

        public static string POP_Port
		{
			get
			{
				if (ConfigurationManager.AppSettings["POP_ACCOUNT_PORT"] != null)
				{
					return ConfigurationManager.AppSettings["POP_ACCOUNT_PORT"].ToString();
				}
				return "";
			}
		}

        public static string POP_Email
		{
			get
			{
				if (ConfigurationManager.AppSettings["POP_ACCOUNT_EMAIL"] != null)
				{
					return ConfigurationManager.AppSettings["POP_ACCOUNT_EMAIL"].ToString();
				}
				return "";
			}
		}

        public static string POP_Password
		{
			get
			{
				if (ConfigurationManager.AppSettings["POP_ACCOUNT_PASS"] != null)
				{
					return ConfigurationManager.AppSettings["POP_ACCOUNT_PASS"].ToString();
				}
				return "";
			}
		}

		#endregion

		#region SMTP Constants

		public static string SMTP_Host
		{
			get
			{
				if (ConfigurationManager.AppSettings["SMTP_ACCOUNT_HOST"] != null)
				{
					return ConfigurationManager.AppSettings["SMTP_ACCOUNT_HOST"].ToString();
				}
				return "";
			}
		}

        public static string SMTP_Port
		{
			get
			{
				if (ConfigurationManager.AppSettings["SMTP_ACCOUNT_PORT"] != null)
				{
					return ConfigurationManager.AppSettings["SMTP_ACCOUNT_PORT"].ToString();
				}
				return "";
			}
		}

        public static string SMTP_Email
		{
			get
			{
				if (ConfigurationManager.AppSettings["SMTP_ACCOUNT_EMAIL"] != null)
				{
					return ConfigurationManager.AppSettings["SMTP_ACCOUNT_EMAIL"].ToString();
				}
				return "";
			}
		}

        public static string SMTP_Password
		{
			get
			{
				if (ConfigurationManager.AppSettings["SMTP_ACCOUNT_PASS"] != null)
				{
					return ConfigurationManager.AppSettings["SMTP_ACCOUNT_PASS"].ToString();
				}
				return "";
			}
		}

		#endregion

		#region Exchange Constants

		public static string EXCHANGE_SERVICE
		{
			get
			{
				if (ConfigurationManager.AppSettings["EXCHANGE_SERVICE"] != null)
				{
					return ConfigurationManager.AppSettings["EXCHANGE_SERVICE"].ToString();
				}
				return "";
			}
		}

		public static string EXCHANGE_ACCOUNT_EMAIL
		{
			get
			{
				if (ConfigurationManager.AppSettings["EXCHANGE_ACCOUNT_EMAIL"] != null)
				{
					return ConfigurationManager.AppSettings["EXCHANGE_ACCOUNT_EMAIL"].ToString();
				}
				return "";
			}
		}

		public static string EXCHANGE_ACCOUNT_PASS
		{
			get
			{
				if (ConfigurationManager.AppSettings["EXCHANGE_ACCOUNT_PASS"] != null)
				{
					return ConfigurationManager.AppSettings["EXCHANGE_ACCOUNT_PASS"].ToString();
				}
				return "";
			}
		}

		#endregion
	}
}