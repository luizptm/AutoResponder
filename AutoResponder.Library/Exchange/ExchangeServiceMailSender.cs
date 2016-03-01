using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Exchange.WebServices.Data;

namespace AutoResponder.Library.Exchange
{
	public class ExchangeServiceMailSender
	{
		private readonly string _username;
		private readonly string _password;
		private readonly string _exchangeUrl;

		public ExchangeServiceMailSender(string username = "", string password = "", string exchangeUrl = "")
		{
			if (username == "")
				_username = Constants.EXCHANGE_ACCOUNT_EMAIL;
			else
				_username = username;
			if (password == "")
				_password = Constants.EXCHANGE_ACCOUNT_PASS;
			else
				_username = password;
			if (exchangeUrl == "")
				_exchangeUrl = Constants.EXCHANGE_SERVICE;
			else
				_username = exchangeUrl;
		}

		public String Send(String subject, String body, String userName, String userEmail, String IDUser = "", String IDTemplate = "")
		{
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			service.Credentials = new WebCredentials(_username, _password);
			service.Url = new Uri(_exchangeUrl);
			
			EmailAddress defaultSender = new EmailAddress(Constants.DEFAULT_SENDER_EMAIL);

			EmailMessage message = new EmailMessage(service);
			message.From = defaultSender;
			message.Sender = defaultSender;
			message.ToRecipients.Add(new EmailAddress(userName, userEmail));
			message.Subject = subject;
			message.Body = body;

			ExtendedPropertyDefinition IDUserHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders,  
                                                                                                   "IDUser", MapiPropertyType.String); 
            ExtendedPropertyDefinition IDTemplateHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders,  
                                                                                                   "IDTemplate", MapiPropertyType.String);  
			message.SetExtendedProperty(IDUserHeader, IDUser);
			message.SetExtendedProperty(IDTemplateHeader, IDTemplate);
			
			message.SendAndSaveCopy();

			return "OK";
		}

		public void SetOutOfOffice()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			service.Credentials = new WebCredentials(_username, _password);
			service.Url = new Uri(_exchangeUrl);

			OofSettings userOOF = new OofSettings();

            // Select the OOF status to be a set time period.
            userOOF.State = OofState.Scheduled;

            // Select the time period to be OOF
            userOOF.Duration = new TimeWindow(DateTime.Now, DateTime.Now.AddMinutes(5));

            // Select the external audience that will receive OOF messages.
            userOOF.ExternalAudience = OofExternalAudience.All;

            // Select the OOF reply for your internal audience.
            userOOF.InternalReply = new OofReply("I'm currently out of office. Please contact my manager for critical issues. Thanks!");

            // Select the OOF reply for your external audience.
            userOOF.ExternalReply = new OofReply("I am currently out of the office but will reply to emails when I return. Thanks!");

            // Set the selected values. This method will result in a call to the Exchange Server.
            service.SetUserOofSettings(Constants.EXCHANGE_ACCOUNT_EMAIL, userOOF);
        }
	}
}