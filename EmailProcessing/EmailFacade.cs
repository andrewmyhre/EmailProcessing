using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class EmailFacade
    {
        private readonly string _templateLocation;
        private readonly string _pickupLocation;
        private IEmailTemplateManager templateManager = null;
        private ITemplateProcessor templateProcessor = null;
        private IEmailPackageSerialiser _packageSerialiser = null;

        public EmailFacade(string templateLocation, string pickupLocation)
        {
            _templateLocation = templateLocation;
            _pickupLocation = pickupLocation;

            EnsureFolderExists(_templateLocation);
            EnsureFolderExists(_pickupLocation);

            templateManager = new EmailTemplateManager(templateLocation);

            templateProcessor = new TemplateProcessor();

            _packageSerialiser = new EmailPackageSerialiser();
        }

        private void EnsureFolderExists(string templateLocation)
        {
            if (!Directory.Exists(templateLocation)) Directory.CreateDirectory(templateLocation);
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
            var packageId = Guid.NewGuid();
            foreach (var attachment in fileAttachments)
            {
                package.Attachments.Add(attachment.FullName);
            }

            var xml = _packageSerialiser.Serialise(package);
            File.WriteAllText(Path.Combine(_pickupLocation, packageId.ToString() + ".xml"), xml);
        }

        public void LoadTemplates()
        {
            templateManager.LoadTemplates();
        }
    }

    public static class EmailFacadeFactory
    {
        public static EmailFacade CreateFromConfiguration()
        {
            var configuration =
                EmailProcessingConfigurationManager.Section;
            return new EmailFacade(configuration.TemplateLocation, configuration.PickupLocation);
        }
    }
}
