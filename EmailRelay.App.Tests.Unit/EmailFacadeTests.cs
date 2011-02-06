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
        [Test]
        public void SendAnEmail()
        {
            EmailFacade ef = new EmailFacade(Environment.CurrentDirectory, 
                EmailProcessingConfigurationManager.Section.PickupLocation);
            ef.LoadTemplates();
            ef.AddEmailToQueue(new[]
            {
                "andrew.myhre@gmail.com"
            }, "templateName", 
            new Dictionary<string, string>(), 
            new FileInfo[0]);
        }
    }
}