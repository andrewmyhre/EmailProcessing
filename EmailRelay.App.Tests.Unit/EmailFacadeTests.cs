using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    public class EmailFacadeTests
    {
        // these are really integration tests but oh whatever

        [Test]
        public void SendAnEmail()
        {
            EmailFacade ef = new EmailFacade(Environment.CurrentDirectory, 
                EmailProcessingConfigurationManager.Section.PickupLocation,
                new NoRelayEmailSender(Path.Combine(Environment.CurrentDirectory, "delivered"),
                    Path.Combine(Environment.CurrentDirectory, "failed")));
            ef.LoadTemplates();
            ef.AddEmailToQueue(new[]
            {
                "andrew.myhre@gmail.com"
            }, "templateName", 
            new Dictionary<string, string>(), 
            new FileInfo[0]);
        }

        [Test]
        public void SendAnEmailWithAttachment()
        {
            EmailFacade ef = new EmailFacade(Path.Combine(Environment.CurrentDirectory, "templates"),
                Path.Combine(Environment.CurrentDirectory, "pickup"), 
                new NoRelayEmailSender(Path.Combine(Environment.CurrentDirectory, "delivered"),
                    Path.Combine(Environment.CurrentDirectory, "failed")));
            ef.LoadTemplates();
            ef.AddEmailToQueue(new[]
            {
                "andrew.myhre@gmail.com"
            }, "templateName",
            new Dictionary<string, string>(),
            new FileInfo[] { new FileInfo(Path.Combine(Environment.CurrentDirectory, "attachment.jpg")), });
        }

        [Test]
        public void SendAnEmailWithAttachmentUsingAmazon()
        {
            EmailFacade ef = new EmailFacade(Path.Combine(Environment.CurrentDirectory, "templates"),
                Path.Combine(Environment.CurrentDirectory, "pickup"),
                new AmazonSESEmailSender(Path.Combine(Environment.CurrentDirectory, "delivered"),
                    Path.Combine(Environment.CurrentDirectory, "failed")));
            ef.LoadTemplates();
            ef.AddEmailToQueue(new[]
            {
                "andrew.myhre@gmail.com"
            }, "templateName",
            new Dictionary<string, string>(),
            new FileInfo[] { new FileInfo(Path.Combine(Environment.CurrentDirectory, "attachment.jpg")), });
        }
    }
}