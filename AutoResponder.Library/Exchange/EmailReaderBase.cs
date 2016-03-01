using System;
using System.Net.Mail;
using System.Threading;

namespace AutoResponder.Library.Exchange
{
	/// <summary>
	/// 
	/// </summary>
    public abstract class EmailReaderBase : IEmailReader
    {
        protected abstract MailMessage DoQuery(string subject = "");

        public abstract void DeleteOldMessages(DateTime beforeDate = default(DateTime));

        protected DateTime FixupBeforeDate(DateTime beforeDate)
        {
            return beforeDate == default(DateTime) 
                ? DateTime.Now.AddDays(-7) 
                : beforeDate;
        }

        /// <summary>
        /// Searches for an email with the gtiven subject. Tries it a number of times if not found.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="waitBeforeNextTryInMiliseconds">Wait time in milliseconds, defaults to 500</param>
        /// <returns></returns>
        public MailMessage GetEmailBySubject(string subject, int numberOfTries = 3, int waitBeforeNextTryInMiliseconds = 500)
        {
            int triesLeft = numberOfTries;
            MailMessage result = null;
            while (result == null && triesLeft > 0)
            {
                result = DoQuery(subject);
                triesLeft--;
                if (result == null && triesLeft > 0) Thread.Sleep(waitBeforeNextTryInMiliseconds);
            }
            return result;
        }
    }
}