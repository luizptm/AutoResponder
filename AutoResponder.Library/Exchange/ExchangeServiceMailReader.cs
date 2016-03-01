using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using AutoResponder.Models.Entity;
using Microsoft.Exchange.WebServices.Data;

namespace AutoResponder.Library.Exchange
{
	public class ExchangeServiceMailReader : EmailReaderBase
	{
		private readonly string _username;
		private readonly string _password;
		private readonly string _exchangeUrl;

		public ExchangeServiceMailReader(string username = "", string password = "", string exchangeUrl = "")
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

		protected override MailMessage DoQuery(string subject = "")
		{
			// open service
			var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			service.Credentials = new WebCredentials(_username, _password);
			service.Url = new Uri(_exchangeUrl);

			// query
			var filter = new SearchFilter.ContainsSubstring(ItemSchema.Subject, subject);
			FindItemsResults<Item> findResults = service.FindItems(
				WellKnownFolderName.Inbox,
				filter,
				new ItemView(10));

			var item = findResults.ToList().OrderByDescending(_ => _.DateTimeSent).FirstOrDefault();
			if (item != null)
			{
				var itempropertyset = new PropertySet(BasePropertySet.FirstClassProperties);
				itempropertyset.RequestedBodyType = BodyType.Text;
				item.Load(itempropertyset);
			}

			var email = item as EmailMessage;

			return (email != null)
					   ? new MailMessage(email.Sender.Address, email.ReceivedBy.Address, email.Subject, email.Body)
					   : null;
		}

		/// <summary>
		/// Deletes old messages received before a given date
		/// </summary>
		/// <param name="beforeDate">The before date.</param>
		public override void DeleteOldMessages(DateTime beforeDate = default(DateTime))
		{
			beforeDate = FixupBeforeDate(beforeDate);

			// open service
			var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			service.Credentials = new WebCredentials(_username, _password);
			service.Url = new Uri(_exchangeUrl);

			var filter = new SearchFilter.IsLessThan(ItemSchema.DateTimeReceived, beforeDate);

			var results = service.FindItems(
				WellKnownFolderName.Inbox,
				filter,
				new ItemView(100)).Select(_ => _.Id).ToList();

			if (results.Count > 0) service.DeleteItems(results, DeleteMode.HardDelete, null, null);
		}

		/// <summary>
        /// Get bounce from email messages.
		/// Source: http://code.msdn.microsoft.com/Exchange-2013-Access-96494465
        /// </summary>
		public String GetBounces()
		{
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			service.Credentials = new WebCredentials(_username, _password);
			service.Url = new Uri(_exchangeUrl);

			// Defines the extended property that contains the entire set of Internet message headers.
			ExtendedPropertyDefinition PR_TRANSPORT_MESSAGE_HEADERS = new ExtendedPropertyDefinition(0x007D, MapiPropertyType.String);

			// Defines a property set that contains the entire set of Internet message headers.
			PropertySet propsAllInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly, PR_TRANSPORT_MESSAGE_HEADERS);

			// Defines a property set that contains the Internet message headers defined in EmailMessageSchema.InternetMessageHeaders.
			// The Internet message headers returned for this property set are a subset of what is returned for
			// PR_TRANSPORT_MESSAGE_HEADERS. The EmailMessageSchema.InternetMessageHeaders property has been deemphasized. 
			// Use either the schematized Internet message headers, or, if the headers aren't schematized, use
			// PR_TRANSPORT_MESSAGE_HEADERS.
			PropertySet propsSomeInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly, 
																			EmailMessageSchema.Body, 
																			EmailMessageSchema.InternetMessageHeaders);

			// Defines a property set that contains the schematized Internet message headers. 
			PropertySet propsSchematizedInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly,
																				 EmailMessageSchema.InternetMessageId, // Message-ID
																				 EmailMessageSchema.ConversationIndex, // Thread-Index
																				 EmailMessageSchema.ConversationTopic, // Thread-Topic
																				 EmailMessageSchema.From, // From
																				 EmailMessageSchema.ToRecipients, // To
																				 EmailMessageSchema.Body, // Body
																				 EmailMessageSchema.CcRecipients, // Cc
																				 EmailMessageSchema.ReplyTo, // Reply-To
																				 EmailMessageSchema.InReplyTo, // In-Reply-To
																				 EmailMessageSchema.References, // References
																				 EmailMessageSchema.Subject, // Subject
																				 EmailMessageSchema.DateTimeSent, // Date
																				 EmailMessageSchema.Sender); // Sender

			ExtendedPropertyDefinition IdUserDefinition = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings,
                                                                                        "IDUser",
                                                                                        MapiPropertyType.String);
			ExtendedPropertyDefinition IdTemplateDefinition = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings,
                                                                                        "IdTemplate",
                                                                                        MapiPropertyType.String);
			
			ItemView view = new ItemView(Int32.MaxValue);
			view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, IdUserDefinition, IdTemplateDefinition);

			// Find the first email message in the Inbox. This results in a FindItem operation call to EWS.
			FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, view);
			int totalBounces = 0;
			foreach (Item item in results)
			{
				// Verify that the item is an email message.
				if (item is EmailMessage)
				{
					// Cast the item to an email message.
					EmailMessage msg = item as EmailMessage;

					if (!msg.Subject.Contains("Undeliverable"))
					{
						continue;
					}
					
					// Load the schematized Internet message headers into the corresponding EmailMessage properties.
					// This results in a GetItem operation call to EWS.
					msg.Load(propsSchematizedInternetMessageHeaders);

					// Load the schematized Internet message headers into the corresponding EmailMessage properties.
					// This results in a GetItem operation call to EWS.
					msg.Load(propsSomeInternetMessageHeaders);

					int IDUser = 0;
					int IDTemplate = 0;
					if (msg.InternetMessageHeaders.Count > 0)
					{
						foreach (InternetMessageHeader hdr in msg.InternetMessageHeaders)
						{
							if (hdr.Name == "IDUser")
							{
								IDUser = Int32.Parse(hdr.Value);
							}
							if (hdr.Name == "IDTemplate")
							{
								IDTemplate = Int32.Parse(hdr.Value);
							}
						}
					}
					if (msg.ExtendedProperties.Count > 0)
					{
						foreach (ExtendedProperty prop in msg.ExtendedProperties)
						{
							if (prop.PropertyDefinition.Name == "IDUser")
							{
								IDUser = Int32.Parse(prop.Value.ToString());
							}
							if (prop.PropertyDefinition.Name == "IDTemplate")
							{
								IDTemplate = Int32.Parse(prop.Value.ToString());
							}
						}
					}
					
					int padding = 0;
					if (IDUser == 0 && IDTemplate == 0)
					{
						string Body = msg.Body;
						if (Body.Contains("MessageControl"))
						{
							string tag = "<MessageControl ";
							int padding_inicio = Body.IndexOf("<MessageControl ");
							
							string partial = "";
							if (Body.Contains("IDUser"))
							{
								string att = "IDUser='";
								partial = Body.Substring(padding_inicio + tag.Length + att.Length);
								int endid = partial.IndexOf("\"");
								string iduser = partial.Substring(0, endid);
								IDUser = int.Parse(iduser);
							}
							if (Body.Contains("IDTemplate"))
							{
								partial = partial.Substring(IDUser.ToString().Length + 2);
								string att = "IDTemplate='";
								partial = partial.Substring(att.Length);
								int endid = partial.IndexOf("\"");
								string idTemplate = partial.Substring(0, endid);
								IDTemplate = int.Parse(idTemplate);
							}
						}
					}

					string content = msg.Body;
					if (content.Contains("iduser"))
					{
						string iduser = content.IndexOf("iduser:").ToString();
						padding = int.Parse(iduser);
						iduser = content.Substring(padding + 7);
						int break_line = iduser.IndexOf("\r\n");
						iduser = iduser.Substring(0, break_line);
						IDUser = int.Parse(iduser);
					}
					if (content.Contains("idtemplate"))
					{
						string idTemplate = content.IndexOf("idtemplate:").ToString();
						padding = int.Parse(idTemplate);
						idTemplate = content.Substring(padding + 11);
						int break_line = idTemplate.IndexOf("\r\n");
						idTemplate = idTemplate.Substring(0, break_line);
						IDTemplate = int.Parse(idTemplate);
					}
					
					if (IDUser != 0 && IDTemplate != 0)
					{
						int bounce = 0;
						Entities db = new Entities();
						BR_AutoResponder_Sending reg = db.BR_AutoResponder_Sending.Where(x => x.UserId == IDUser && x.TemplateId == IDTemplate).FirstOrDefault();
						if (reg != null)
						{
							reg.Bounce = true;
							db.Entry(reg).State = EntityState.Modified;
							bounce = db.SaveChanges();
						}
						totalBounces += bounce;
					}
				}
			}
			return totalBounces.ToString();
		}
	}
}