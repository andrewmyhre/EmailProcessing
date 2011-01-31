using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing;
using log4net.Config;

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
            try
            {
                string pickupPath = ConfigurationManager.AppSettings["pickupLocation"] ??
                                    Path.Combine(Environment.CurrentDirectory, "pickup");
                string deliveredPath = ConfigurationManager.AppSettings["deliveredLocation"] ??
                                       Path.Combine(Environment.CurrentDirectory, "delivered");
                string failedPath = ConfigurationManager.AppSettings["failedLocation"] ??
                                    Path.Combine(Environment.CurrentDirectory, "failed");

                if (!Directory.Exists(pickupPath)) Directory.CreateDirectory(pickupPath);
                if (!Directory.Exists(deliveredPath)) Directory.CreateDirectory(deliveredPath);
                if (!Directory.Exists(failedPath)) Directory.CreateDirectory(failedPath);



                packageSerializer = new EmailPackageSerialiser();
                watcher = new EmailWatcher(pickupPath, packageSerializer);
                sender = new EmailSender(deliveredPath,
                                         failedPath);

                watcher.OnMailToSend += sender.SendMail;

                Console.Clear();
                Console.WriteLine("Watching " + pickupPath + ". Press Esc to stop");
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
