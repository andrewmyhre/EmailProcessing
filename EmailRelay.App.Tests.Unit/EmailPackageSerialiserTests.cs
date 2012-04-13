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
            Assert.That(package.To, Has.Some.InstanceOf<Recipient>().With.Property("EmailAddress").StringMatching("testrecipient@test.com"));
            Assert.That(package.Html.Trim(), Is.StringContaining(@"<p>html body</p>"));
            Assert.That(package.Text.Trim(), Is.StringContaining(@"text body"));
            Assert.That(package.Attachments, Has.Count.EqualTo(1));
            Assert.That(package.Attachments,
                        Has.Some.InstanceOf<Attachment>().With.Property("Path").StringContaining(@"attachment.jpg"));
        }

        [Test]
        public void EmailPackageSerialiser_Serialisation_CanSerialiseAnEmailPackage()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            EmailPackage package = Helpers.SamplePackage();
            string serialised = serialiser.Serialise(package);
            string namespaceName = "http://www.tanash.net/email/package";

            System.Diagnostics.Debug.WriteLine(serialised);

            XDocument doc = XDocument.Parse(serialised);
            var serialisedPackage = doc.Element(XName.Get("emailPackage", namespaceName));
            Assert.That(serialisedPackage.Element(XName.Get("from", namespaceName)).Value, Is.EqualTo(package.From));
            Assert.That(serialisedPackage.Element(XName.Get("subject", namespaceName)).Value, Is.EqualTo(package.Subject));
            Assert.That(serialisedPackage.Element(XName.Get("html", namespaceName)).Value, Is.EqualTo(package.Html));
            Assert.That(serialisedPackage.Element(XName.Get("text", namespaceName)).Value, Is.EqualTo(package.Text));
            Assert.That(serialisedPackage
                            .Element(XName.Get("recipients", namespaceName))
                            .Elements(XName.Get("recipient", namespaceName)).ToList(), 
                            Has.Count.EqualTo(1));
            Assert.That(serialisedPackage
                            .Element(XName.Get("attachments", namespaceName))
                            .Elements(XName.Get("attachment", namespaceName)).ToList(),
                            Has.Count.EqualTo(1));
        }

        [Test]
        public void EmailPackageSerialiser_CanSerialiseAndDeserialiseIntoEqualObject()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            var expected = serialiser.Deserialize(File.ReadAllText("SampleEmailPackage.xml"));
            var serialised = serialiser.Serialise(expected);
            var actual = serialiser.Deserialize(serialised);

            Assert.That(actual.To[0].EmailAddress, Is.EqualTo(expected.To[0].EmailAddress));
            Assert.That(actual.To[0].Name, Is.EqualTo(expected.To[0].Name));
           Assert.That(actual.Attachments[0].Path, Is.EqualTo(expected.Attachments[0].Path));
            Assert.That(actual.From, Is.EqualTo(expected.From));
            Assert.That(actual.Subject, Is.EqualTo(expected.Subject));
            Assert.That(actual.Html, Is.EqualTo(expected.Html));
            Assert.That(actual.Text, Is.EqualTo(expected.Text));
        }
    }
}
