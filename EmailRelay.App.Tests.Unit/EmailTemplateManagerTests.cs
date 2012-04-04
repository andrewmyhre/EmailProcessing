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
            var template = templateParser.Parse(File.ReadAllText("templates\\SampleEmailTemplate.xml"));

            Assert.That(template.Name, Is.StringMatching("templateName"));
            Assert.That(template.Subject, Is.StringMatching("test email subject"));
            Assert.That(template.From, Is.StringMatching("andrew.myhre@gmail.com"));
            Assert.That(template.Html, Is.StringMatching("html body"));
            Assert.That(template.Text, Is.StringMatching("text body"));
            Assert.That(template.Culture, Is.StringMatching("en"));
        }

        [Test]
        public void EmailTemplateManager_WillLoadAllTemplatesInAFolder()
        {
            if (!Directory.Exists("templates\\test"))
                Directory.CreateDirectory("templates\\test");
            DirectoryInfo dir = new DirectoryInfo("templates\\test");
            foreach(var file in dir.GetFiles())
                file.Delete();

            File.Copy("templates\\SampleEmailTemplate.xml", Path.Combine("templates\\test", "template1.xml"));
            File.Copy("templates\\SampleEmailTemplate.xml", Path.Combine("templates\\test", "template2.xml"));
            File.Copy("templates\\SampleEmailTemplate.xml", Path.Combine("templates\\test", "template3.xml"));
            File.Copy("templates\\SampleEmailTemplate.xml", Path.Combine("templates\\test", "template4.xml"));
            File.Copy("templates\\SampleEmailTemplate.xml", Path.Combine("templates\\test", "template5.xml"));

            EmailTemplateManager templateManager = new EmailTemplateManager("templates\\test");
            templateManager.LoadTemplates();

            Assert.That(templateManager.Templates, Has.Count.EqualTo(5));
        }
    }
}
