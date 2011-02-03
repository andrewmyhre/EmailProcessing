using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmailProcessing;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    [Ignore]
    public class AmazonService
    {
        [TestCase("andrew.myhre@gmail.com")]
        public void VerifyEmail(string email)
        {
            AmazonSESEmailSender aSes = new AmazonSESEmailSender();
            aSes.VerifyEmail(email);
        }
    }
}
