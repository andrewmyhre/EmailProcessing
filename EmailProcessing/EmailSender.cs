using System;
using System.IO;

namespace EmailProcessing
{
    public class EmailSender : IEmailSender
    {
        private readonly string _deliveredLocation;
        private readonly string _failedLocation;

        public EmailSender(string deliveredLocation, string failedLocation)
        {
            _deliveredLocation = deliveredLocation;
            _failedLocation = failedLocation;
        }

        public void SendMail(object sender, EmailToSendArgs e)
        {
            Console.WriteLine("send package " + e.Message.Subject);
            string filename = Path.GetFileName(e.PackagePath);
            File.Move(e.PackagePath, Path.Combine(_deliveredLocation, Guid.NewGuid() + ".xml"));
        }
    }
}