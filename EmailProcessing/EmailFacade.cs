using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class EmailFacade
    {
        private IEmailTemplateManager templateManager = null;
        private ITemplateProcessor templateProcessor = null;
        private IEmailPackageSerialiser _packageSerialiser = null;

        public EmailFacade(string templateLocation)
        {
            templateManager = new EmailTemplateManager(templateLocation);
            templateManager.LoadTemplates();

            templateProcessor = new TemplateProcessor();

            _packageSerialiser = new EmailPackageSerialiser();
        }

        public void AddEmailToQueue(string templateName,
            Dictionary<string,string> tokenReplacements,
            FileInfo[] fileAttachments)
        {
            var template = templateManager.Templates.Where(t => t.Name == templateName).FirstOrDefault();
            if (template ==null)
                throw new ArgumentException("No such template " + templateName);

            NameValueCollection nvc = new NameValueCollection();
            foreach(var token in tokenReplacements)
                nvc.Add(token.Key, token.Value);

            var package = templateProcessor.CreatePackageFromTemplate(template,
                                                                      nvc);

            _packageSerialiser.Serialise(package);
        }
    }
}
