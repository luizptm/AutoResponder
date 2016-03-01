using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using AutoResponder.Library;
using AutoResponder.Library.Exchange;
using AutoResponder.Library.POP3;
using AutoResponder.Models.Entity;
using AutoResponder.Models.Repository;
using NLog;

namespace AutoResponder.Library.Main
{
	// The Dangers of Implementing Recurring Background Tasks In ASP.NET
	// http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx/
	public class SendingScheduledEmail : IRegisteredObject
	{
		#region Attributes

		private UnitOfWork uow = null;

		private Entities db = new Entities();
		
		private Repository<BR_AutoResponder_SendingList> repositorySendingList = null;
		private Repository<BR_AutoResponder_Sending> repositorySending = null;
		private Repository<BR_AutoResponder_Tag> repositoryTag = null;
		private Repository<BR_AutoResponder_Template> repositoryTemplate = null;
		private Repository<BR_Users> repositoryUsers = null;
		private Repository<BR_AutoResponder_UserEntry> repositoryUserEntry = null;

		private List<BR_AutoResponder_Sending> sendings = null;

		private readonly object _lock = new object();
		private bool _shuttingDown;

		Logger logger = LogManager.GetLogger("*");

		#endregion

		#region Methods

		public SendingScheduledEmail()
		{
			this.uow = new UnitOfWork();
			
			this.repositorySendingList = new Repository<BR_AutoResponder_SendingList>(uow);
			this.repositorySending = new Repository<BR_AutoResponder_Sending>(uow);
			this.repositoryTag = new Repository<BR_AutoResponder_Tag>(uow);
			this.repositoryTemplate = new Repository<BR_AutoResponder_Template>(uow);
			this.repositoryUsers = new Repository<BR_Users>(uow);
			this.repositoryUserEntry = new Repository<BR_AutoResponder_UserEntry>(uow);

			this.sendings = repositorySending.GetAll().ToList();

			HostingEnvironment.RegisterObject(this);
		}

		public void Stop(bool immediate)
		{
			lock (_lock)
			{
				_shuttingDown = true;
			}
			HostingEnvironment.UnregisterObject(this);
		}

		public void DoWork(int sendingListId = 0)
		{
			lock (_lock)
			{
				if (_shuttingDown)
				{
					return;
				}
				ExecuteTask(sendingListId);
			}
		}

		public String ExecuteTask(int sendingListId = 0, int userId = 0)
		{
			String output = "";
			int sendings = 0;
			int not_sendings = 0;
			List<int> listStatus = new List<int>();
			
			List<BR_AutoResponder_SendingList> sendingLists = this.repositorySendingList.GetAll().ToList();
			List<BR_AutoResponder_UserEntry> userEntriesByList = this.repositoryUserEntry.GetAll().ToList();

			if (sendingListId != 0)
			{
				listStatus = this.InterateInTemplates(userEntriesByList, sendingListId, userId);
			}
			else
			{
				foreach (BR_AutoResponder_SendingList list in sendingLists)
				{
					sendingListId = list.Id;
					listStatus = this.InterateInTemplates(userEntriesByList, sendingListId, userId);
				}
			}
			if (listStatus.Count > 0)
			{
				sendings = listStatus[0];
				not_sendings = listStatus[1];
			}
			if (Constants.USE_POP3 == "true")
			{
				output += "<b>Resultado: via SMTP (" + Constants.SMTP_Host + "):</b><br/>";
			}
			else
			{
				output += "<b>Resultado: via Exchange (" + Constants.EXCHANGE_SERVICE + "):</b><br/>";
			}
			output += "Foram enviados <b>" + sendings + "</b> e-mails";
			if (sendings > 0)
			{
				output += " com sucesso";
			}
			if (sendingListId > 0)
			{
				output += " na lista selecionada";
			}
			output += ".";

			if (not_sendings > 0)
			{
				output += "<br/>Envios com problemas para <b>" + not_sendings + "</b> e-mails.";
				output += "<br/>Para maiores informações, veja o log.";
			}
			return output;
		}

		private List<int> InterateInTemplates(List<BR_AutoResponder_UserEntry> userEntriesByList, int sendingListId, int userId)
		{
			int sendings = 0;
			int not_sendings = 0;
			List<int> listStatus = new List<int>();

			if (userId != 0)//enviar somente para um usuário específico
			{
				userEntriesByList = new List<BR_AutoResponder_UserEntry>();
				BR_AutoResponder_UserEntry userEntry = db.BR_AutoResponder_UserEntry.Where(x => x.UserId == userId).Single();
				userEntriesByList.Add(userEntry);
			}
			else
			{
				userEntriesByList = userEntriesByList.Where(x => x.SendingListId == sendingListId).OrderBy(x => x.EntryDate).ToList(); //TODOS os usuários da lista (COM ou SEM Optin)
					
				BR_AutoResponder_SendingList sendingList = db.BR_AutoResponder_SendingList.Find(sendingListId);
			}

			List<BR_AutoResponder_Template> templatesByList = repositoryTemplate.GetAll().Where(x => x.SendingListId == sendingListId).OrderBy(x => x.Sequence).ToList(); //ordena os templates pela sequência
			foreach (BR_AutoResponder_Template template in templatesByList)
			{
				if (template == null) continue;
				
				foreach (BR_AutoResponder_UserEntry userEntry in userEntriesByList)
				{
					if (userEntry == null) continue;

					bool sendEmail = this.VerifySendingUserStatus(template, userEntry, templatesByList); //verificar se o usuário irá receber e-mail
					if (sendEmail)
					{
						bool result = false;
						userId = userEntry.UserId;
						BR_Users user = userEntry.BR_Users;
						if (template.Sequence == 0)
						{
							Stats stats = new Stats();
							result = stats.SendOptinTemplate(userId.ToString(), sendingListId.ToString());
						}
						else
						{
							int templateId = template.Id;
							string body = template.Body;
							string username = getUserFullName(user);
							body = Stats.InsertStats(body, templateId.ToString(), userId.ToString());
							result = this.SendMail(template.Subject, body, username, user.email, userId.ToString(), templateId.ToString(), sendingListId.ToString());
						}
						if (result == true)
						{
							this.RegisterSending(template, user);
							string msg = getLogger("E-mail ENVIADO", sendingListId.ToString(), template.Id.ToString(), user.idUser.ToString(), user.firstName, user.email);
							logger.Trace(msg);
							sendings++;
						}
						else
						{
							not_sendings++;
						}
					}
				}// foreach
			}

			listStatus.Add(sendings);
			listStatus.Add(not_sendings);
			return listStatus;
		}

		private bool VerifySendingUserStatus(BR_AutoResponder_Template template, BR_AutoResponder_UserEntry userEntry, List<BR_AutoResponder_Template> templatesByList)
		{
			/*
			* 0° envio : envio imediato (o usuário não precisa ter feito o optin)
			* 1º envio : template 1 : data de entrada + intervalo
			* 2º envio : template 2 : data de envio do template anterior + intervalo
			* ...
			*/
			bool sendEmail = false;

			int userId = userEntry.UserId;
			
			List<BR_AutoResponder_Sending> sendingByUser = this.sendings.Where(x => x.TemplateId == template.Id && x.UserId == userId).ToList();
			int sendingByUserCount = sendingByUser.Count();
			if (sendingByUserCount > 0) //verifica se o template já foi enviado para o usuário corrente
			{
				return sendEmail; //não enviar o e-mail!
			}

			if (template.Sequence == 0) //template 0 : envio imediato
			{
				sendEmail = true; //envio imediato!
			}
			else if (template.Sequence >= 1 && userEntry.Optin == true) 
			{
				/* ***
				* Usar a data de envio do template anterior na sequência, enviado para o usuário
				* ***/
				BR_AutoResponder_Template previous_template = templatesByList.Where(x => x.Sequence == template.Sequence - 1).SingleOrDefault();
				if (previous_template != null)
				{
					List<BR_AutoResponder_Sending> previousSendingsByUser = this.sendings.Where(x => x.TemplateId == previous_template.Id && x.UserId == userId).ToList();
					if (previousSendingsByUser.Count > 0)
					{
						BR_AutoResponder_Sending previousSendingByUser = previousSendingsByUser.First();
						if (previousSendingByUser != null)
						{
							int difference = 0;
							DateTime hoje = DateTime.Now;
							hoje = new DateTime(hoje.Year, hoje.Month, hoje.Day);
							DateTime date = new DateTime();
							int interval = (int)template.Interval;

							if (template.Sequence == 1 && userEntry.Optin == true) // 1º envio : template 1 : data de entrada + intervalo
							{
								DateTime userEntryDate = new DateTime(userEntry.EntryDate.Year, userEntry.EntryDate.Month, userEntry.EntryDate.Day);
								date = userEntryDate.AddDays(interval);
								difference = hoje.Subtract(date).Days;
								if (difference >= 0)
								{
									sendEmail = true;
								}
							}
							else if (template.Sequence > 1 && userEntry.Optin == true) // ex.: 2º envio : template 2 : data de envio do template anterior + intervalo do template corrente
							{
								DateTime? sentDateNullable = previousSendingByUser.SentDate.Value;
								DateTime sentDate = new DateTime(previousSendingByUser.SentDate.Value.Year, previousSendingByUser.SentDate.Value.Month, previousSendingByUser.SentDate.Value.Day);
								date = sentDate.AddDays(interval);
								difference = hoje.Subtract(date).Days;
								if (difference >= 0)
								{
									sendEmail = true;
								}
							}
						}
					}
				}
			}
			return sendEmail;
		}

		public bool SendMail(String subject, String body, String userName, String userEmail, String IDUser = "", String IDTemplate = "", String IDList = "")
		{
			string returnSend = "";
			try
			{
				if (Constants.USE_POP3 == "true")
				{
					returnSend = Pop3SendMail.SendMail(subject, body, userName, userEmail, IDUser, IDTemplate);//code for pop3
					//returnSend = Mandril.SendMail(subject, body, userName, userEmail, IDUser, IDTemplate);//code for pop3
				}
				else
				{
					ExchangeServiceMailSender s = new ExchangeServiceMailSender();
					returnSend = s.Send(subject, body, userName, userEmail, IDUser, IDTemplate);
				}
			}
			catch (Exception ex)
			{
				string msg = getLogger("Error log message", IDList, IDTemplate, IDUser, userName, userEmail);
				logger.Error(ex.Message + "[ " + msg  + " ]");
				return false;
			}
			if (returnSend == "OK")
				return true;
			else
				return false;
		}

		public void RegisterSending(BR_AutoResponder_Template template, BR_Users user)
		{
			BR_AutoResponder_Sending newSending = new BR_AutoResponder_Sending();
			newSending.UserId = user.idUser;
			newSending.TemplateId = template.Id;
			newSending.SentDate = DateTime.Now;
			newSending.CREATION_DATE = DateTime.Now;
			repositorySending.Post(newSending);
			uow.Save();
		}

		private String getLogger(string message, string listingId = "", string templateId = "", string idUser = "", string user = "", string email = "")
		{
			//String format = message + " : list Id: {0},template Id: {1},ID user: {2}, usuario: {3},email: {4}";
			String format = "email: {4}";
			String output = String.Format(format, listingId, templateId, idUser, user, email);
			return output;
		}
		
		public static string getUserFullName(BR_Users user)
		{
			user.firstName =  user.firstName == null ? "" : user.firstName.Trim();
			user.middleName = user.middleName == null ? "" : user.middleName.Trim();
			user.lastName = user.lastName == null ? "" : user.lastName.Trim();
			user.middleName = String.IsNullOrEmpty(user.middleName) ? " " + user.middleName : "";
			user.lastName = String.IsNullOrEmpty(user.lastName) ? " " + user.lastName : "";

			return string.Format("{0}{1}{2}", user.firstName, user.middleName, user.lastName);
		}

		#endregion
	}
}