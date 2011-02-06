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
<<<<<<< HEAD
            logger.Debug("send package " + e.Message.Subject);
            
=======
            Console.WriteLine("send package " + e.Message.Subject);

            string outputLocation = DeliveredLocation;
            if (e.SendingFailed)
                outputLocation = FailedLocation;

>>>>>>> df2bd0ff731acca04dbde3c609ca8540577f7d39
            // create a directory for the package
            string deliveryPath = Path.Combine(outputLocation, Guid.NewGuid().ToString());
            Directory.CreateDirectory(deliveryPath);

            // copy attachments to delivery folder
            if (e.Message.Attachments != null)
            {
                foreach (string attachment in e.Message.Attachments)
                    File.Copy(attachment, Path.Combine(deliveryPath, Path.GetFileName(attachment)));
            }

            // copy the email package to delivery folder
            File.Move(e.PackagePath, Path.Combine(deliveryPath, Path.GetFileName(e.PackagePath)));
        }
    }
}