using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.SimpleEmail.Model;
using EmailProcessing;
using log4net.Config;
using Amazon;

namespace EmailRelay.App
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            IEmailPackageSerialiser packageSerializer = null;
            IEmailWatcher watcher = null;
            IEmailSender sender = null;
            EmailProcessingConfigurationSection configuration = EmailProcessingConfigurationManager.Section;

            Console.Clear();
            var client = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(configuration.Amazon.Key,
                                                                               configuration.Amazon.Secret);
            var emails = client.ListVerifiedEmailAddresses(new ListVerifiedEmailAddressesRequest());
            Console.WriteLine("verified emails:");
            foreach(var email in emails.ListVerifiedEmailAddressesResult.VerifiedEmailAddresses)
                Console.WriteLine(email);

            try
            {
                if (!Directory.Exists(configuration.PickupLocation)) Directory.CreateDirectory(configuration.PickupLocation);
                if (!Directory.Exists(configuration.DeliveredLocation)) Directory.CreateDirectory(configuration.DeliveredLocation);
                if (!Directory.Exists(configuration.FailedLocation)) Directory.CreateDirectory(configuration.FailedLocation);

                
                packageSerializer = new EmailPackageSerialiser();
                watcher = new EmailWatcher(packageSerializer, configuration);
                sender = EmailSenderFactory.CreateSenderFromConfiguration();

                watcher.OnMailToSend += sender.SendMail;

                Console.WriteLine("Watching " + configuration.PickupLocation + ". Press Esc to stop");
                watcher.StartWatching();

                ConsoleKeyInfo key = new ConsoleKeyInfo();
                while (key.Key != ConsoleKey.Escape)
                    key = Console.ReadKey();
            }
            finally
            {
                if (watcher != null)
                    watcher.Dispose();
            }
        }
    }
}
