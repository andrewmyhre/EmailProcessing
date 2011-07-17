using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class EmailFacade : IEmailFacade
    {
        private readonly string _templateLocation;
        private readonly string _pickupLocation;
        private IEmailTemplateManager templateManager = null;
        private ITemplateProcessor templateProcessor = null;
        private IEmailPackageSerialiser _packageSerialiser = null;
        private IEmailSender _sender = null;

        public EmailFacade(string templateLocation, string pickupLocation, IEmailSender sender)
        {
            _templateLocation = templateLocation;
            _pickupLocation = pickupLocation;
            _sender = sender;

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

        public void Send<T>(string to, string templateName, T model)
        {
            Send(new[] {to}, templateName, model);
        }

        public void Send<T>(string[] to, string templateName,
            T model)
        {
            Send(to, templateName, model, null);
        }

        public void Send<T>(string[] to, string templateName,
            T model,
            FileInfo[] fileAttachments)
        {
            var template = templateManager.Templates.Where(t => t.Name == templateName).FirstOrDefault();
            if (template == null)
                throw new ArgumentException("No such template " + templateName);

            var package = templateProcessor.CreatePackageFromTemplate(template, model);
            package.To = new RecipientList(to);
            var packageId = string.Format("{0}-{1}", templateName, DateTime.Now.Ticks);

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                {
                    package.Attachments.Add(attachment.FullName);
                }
            }

            var xml = _packageSerialiser.Serialise(package);
            string packagePath = Path.Combine(_pickupLocation, packageId.ToString() + ".xml");
            File.WriteAllText(packagePath, xml);
          
        }

        public void Send(string to, string templateName, Dictionary<string, string> tokenReplacements)
        {
            Send(new string[] { to }, templateName, tokenReplacements, null);
        }

        public void Send(string[] to, string templateName, Dictionary<string, string> tokenReplacements)
        {
            Send(to, templateName, tokenReplacements, null);
        }

        public void Send(string[] to, string templateName,
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
            package.To = new RecipientList(to);
            var packageId = Guid.NewGuid();

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                {
                    package.Attachments.Add(attachment.FullName);
                }
            }

            var xml = _packageSerialiser.Serialise(package);
            string packagePath = Path.Combine(_pickupLocation, packageId.ToString() + ".xml");
            File.WriteAllText(packagePath, xml);
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
            IEmailSender sender = null;
            Type senderType = Type.GetType(EmailProcessingConfigurationManager.Section.EmailSenderType.Type);
            sender = Activator.CreateInstance(senderType, null) as IEmailSender;
            return new EmailFacade(
                configuration.TemplateLocation, 
                configuration.PickupLocation,
                sender);
        }
    }
}
