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
            Assert.That(package.Html.Trim(), Is.StringContaining(@"<p>html body</p>"));
            Assert.That(package.Text.Trim(), Is.StringContaining(@"text body"));
            Assert.That(package.Tokens, Has.Count.EqualTo(3));
            Assert.That(package.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("subjectToken"));
            Assert.That(package.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("htmlBodyToken"));
            Assert.That(package.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("textBodyToken"));
        }

        [Test]
        public void EmailPackageSerialiser_Serialisation_CanSerialiseAnEmailPackage()
        {
            EmailPackageSerialiser serialiser = new EmailPackageSerialiser();
            EmailPackage package = SamplePackage();
            string serialised = serialiser.Serialise(package);
            string jgnamespace= "http://www.justgiving.com/xml/";

            XDocument doc = XDocument.Parse(serialised);
            Assert.That(doc.Element(XName.Get("emailPackage", jgnamespace)).Element(XName.Get("from", jgnamespace)).Value, Is.EqualTo(package.From));
            Assert.That(doc.Element(XName.Get("emailPackage", jgnamespace)).Element(XName.Get("subject", jgnamespace)).Value, Is.EqualTo(package.Subject));
            Assert.That(doc.Element(XName.Get("emailPackage", jgnamespace)).Element(XName.Get("html", jgnamespace)).Value, Is.EqualTo(package.Html));
            Assert.That(doc.Element(XName.Get("emailPackage", jgnamespace)).Element(XName.Get("text", jgnamespace)).Value, Is.EqualTo(package.Text));
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

        private EmailPackage SamplePackage()
        {
            return new EmailPackage()
                {
                    From = "test@test.com",
                    Html = "test html {token1}",
                    Text = "test text {token2}",
                    Subject = "test subject {token3}",
                    Tokens = new TokenList {"token1", "token2", "token3"}
                };
        }
    }
}
