using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using EmailProcessing;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    public class TemplateProcessorTests
    {
        [Test]
        public void TemplateProcessor_TokensAreReplaced()
        {
            EmailTemplate template = new EmailTemplate()
                                         {
                                             Subject = "subject {token1}",
                                             Text = "text {token2}",
                                             Html = "html {token3}"
                                         };
            TemplateProcessor processor = new TemplateProcessor();

            var package = processor.CreatePackageFromTemplate(template,
                                    new NameValueCollection()
                                        {
                                            {"token1", "replacementvalue1"},
                                            {"token2", "replacementvalue2"},
                                            {"token3", "replacementvalue3"}
                                        });

            Assert.That(package.Subject, Is.StringMatching("subject replacementvalue1"));
            Assert.That(package.Text, Is.StringMatching("text replacementvalue2"));
            Assert.That(package.Html, Is.StringMatching("html replacementvalue3"));
        }
    }
}
