using System;
using System.Net.Mail;

namespace AutoResponder.Library.Exchange
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailReader
    {
        /// <summary>
        /// Searches for an email with the gtiven subject. Tries it a number of times if not found.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="waitBeforeNextTry"></param>
        /// <returns></returns>
        MailMessage GetEmailBySubject(string subject, int numberOfTries = 1, int waitBeforeNextTry = 100);

        /// <summary>
        /// Deletes old messages received before a given date
        /// </summary>
        /// <param name="beforeDate">The before date, default is default(DateTime) which implementers need to resolve to Now - 1 month.</param>
        void DeleteOldMessages(DateTime beforeDate = default(DateTime)); 
    }
}