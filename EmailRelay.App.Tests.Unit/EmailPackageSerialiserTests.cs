using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using EmailProcessing;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    public class EmailPackageSerialiserTests
    {
        [Test]
        public void EmailPackageSerialiser_Deserialisation_CanHydrateAnEmailPackage()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            var package = serialiser.Deserialize(File.ReadAllText("SampleEmailPackage.xml"));

            Assert.That(package.From, Is.EqualTo("testfrom@server.com"));
            Assert.That(package.Subject, Is.EqualTo("test email subject: {subjectToken}"));
            Assert.That(package.To, Has.Count.EqualTo(1));
            Assert.That(package.To, Has.Some.StringMatching("testrecipient@test.com"));
            Assert.That(package.Html.Trim(), Is.StringContaining(@"<p>html body</p>"));
            Assert.That(package.Text.Trim(), Is.StringContaining(@"text body"));
            Assert.That(package.Attachments, Has.Count.EqualTo(1));
            Assert.That(package.Attachments, Has.Some.StringContaining("D:\\workspace\\git\\EmailSystem\\EmailRelay.App.Tests.Unit\\attachment.jpg"));
        }

        [Test]
        public void EmailPackageSerialiser_Serialisation_CanSerialiseAnEmailPackage()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            EmailPackage package = Helpers.SamplePackage();
            string serialised = serialiser.Serialise(package);
            string jgnamespace = "http://www.tanash.net/email/package";

            XDocument doc = XDocument.Parse(serialised);
            var serialisedPackage = doc.Element(XName.Get("emailPackage", jgnamespace));
            Assert.That(serialisedPackage.Element(XName.Get("from", jgnamespace)).Value, Is.EqualTo(package.From));
            Assert.That(serialisedPackage.Element(XName.Get("subject", jgnamespace)).Value, Is.EqualTo(package.Subject));
            Assert.That(serialisedPackage.Element(XName.Get("html", jgnamespace)).Value, Is.EqualTo(package.Html));
            Assert.That(serialisedPackage.Element(XName.Get("text", jgnamespace)).Value, Is.EqualTo(package.Text));
            Assert.That(serialisedPackage
                            .Element(XName.Get("recipients", jgnamespace))
                            .Elements(XName.Get("address", jgnamespace)).ToList(), 
                            Has.Count.EqualTo(1));
            Assert.That(serialisedPackage
                            .Element(XName.Get("attachments", jgnamespace))
                            .Elements(XName.Get("attachment", jgnamespace)).ToList(),
                            Has.Count.EqualTo(1));
        }

        [Test]
        public void EmailPackageSerialiser_CanSerialiseAndDeserialiseIntoEqualObject()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            var package = serialiser.Deserialize(File.ReadAllText("SampleEmailPackage.xml"));
            var serialised = serialiser.Serialise(package);
            var deserialised = serialiser.Deserialize(serialised);

            foreach(var propertyInfo in typeof(EmailPackage).GetProperties())
            {
                object expected = propertyInfo.GetValue(package, null);
                object actual = propertyInfo.GetValue(deserialised, null);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
