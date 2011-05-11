using System;
using System.IO;
using log4net;

namespace EmailProcessing
{
    public abstract class EmailSender : IEmailSender
    {
        private static ILog logger = LogManager.GetLogger(typeof (EmailSender));
        protected readonly string DeliveredLocation;
        protected readonly string FailedLocation;

        public EmailSender()
        {
            DeliveredLocation = EmailProcessingConfigurationManager.Section.DeliveredLocation;
            FailedLocation = EmailProcessingConfigurationManager.Section.FailedLocation;
        }

        public EmailSender(string deliveredLocation, string failedLocation)
        {
            DeliveredLocation = deliveredLocation;
            FailedLocation = failedLocation;
        }

        public virtual void SendMail(object sender, EmailToSendArgs e)
        {

        }
    }
}