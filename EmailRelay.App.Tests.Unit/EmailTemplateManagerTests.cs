using System;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    public class EmailTemplateManagerTests
    {
        [Test]
        public void EmailTemplateManager_LoadingTemplates_CanLoadATemplateXml()
        {
            ITemplateParser templateParser = new TemplateParser();
            var template = templateParser.Parse(File.ReadAllText("SampleEmailTemplate.xml"));

            Assert.That(template.Name, Is.StringMatching("templateName"));
            Assert.That(template.Subject, Is.StringMatching("test email subject"));
            Assert.That(template.From, Is.StringMatching("andrew.myhre@gmail.com"));
            Assert.That(template.Html, Is.StringMatching("html body"));
            Assert.That(template.Text, Is.StringMatching("text body"));
            Assert.That(template.Tokens, Has.Count.EqualTo(3));
            Assert.That(template.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("subjectToken"));
            Assert.That(template.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("htmlBodyToken"));
            Assert.That(template.Tokens, Has.Some.InstanceOf(typeof(string)).And.EqualTo("textBodyToken"));
        }

        [Test]
        public void EmailTemplateManager_WillLoadAllTemplatesInAFolder()
        {
            if (!Directory.Exists("templates"))
                Directory.CreateDirectory("templates");
            DirectoryInfo dir = new DirectoryInfo("templates");
            foreach(var file in dir.GetFiles())
                file.Delete();

            File.Copy("SampleEmailTemplate.xml", Path.Combine("templates", "template1.xml"));
            File.Copy("SampleEmailTemplate.xml", Path.Combine("templates", "template2.xml"));
            File.Copy("SampleEmailTemplate.xml", Path.Combine("templates", "template3.xml"));
            File.Copy("SampleEmailTemplate.xml", Path.Combine("templates", "template4.xml"));
            File.Copy("SampleEmailTemplate.xml", Path.Combine("templates", "template5.xml"));

            EmailTemplateManager templateManager = new EmailTemplateManager("templates");
            templateManager.LoadTemplates();

            Assert.That(templateManager.Templates, Has.Count.EqualTo(5));
        }
    }
}
