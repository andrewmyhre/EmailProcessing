using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing.Configuration;
using log4net;

namespace EmailProcessing
{
    public class EmailFacade : IEmailFacade
    {
        private static ILog logger = LogManager.GetLogger(typeof (EmailFacade));
        private readonly string _templateLocation;
        private readonly string _pickupLocation;
        private readonly IEmailTemplateManager _templateManager = null;
        private ITemplateProcessor templateProcessor = null;
        private IEmailPackageSerialiser _packageSerialiser = null;
        private IEmailPackageRelayer _packageRelayer = null;

        public EmailFacade(EmailBuilderConfigurationSection configuration, IEmailPackageRelayer packageRelayer, IEmailTemplateManager templateManager)
        {
            _templateLocation = Util.DevirtualizePath(configuration.TemplateLocation);

            _pickupLocation = Util.DevirtualizePath(configuration.PickupLocation);
            _packageRelayer = packageRelayer;
            _templateManager = templateManager;

            EnsureFolderExists(_templateLocation);
            EnsureFolderExists(_pickupLocation);

            templateProcessor = new TemplateProcessor();

            _packageSerialiser = new EmailPackageSerialiser();
        }

        private void EnsureFolderExists(string templateLocation)
        {
            if (!Directory.Exists(templateLocation)) Directory.CreateDirectory(templateLocation);
        }

        public void Send<T>(string to, string templateName, T model, string culture = "pl")
        {
            Send(new[] {to}, templateName, model, null, culture);
        }

        public void Send<T>(string[] to, string templateName,
            T model,
            FileInfo[] fileAttachments, string culture = "pl")
        {
            // culture may be in format en-GB
            if (culture.Contains("-"))
                culture = culture.Substring(0, culture.IndexOf("-"));

            logger.DebugFormat("sending email {0}.{1}", templateName, culture);
            var template = _templateManager.Templates.Where(t => t.Name == templateName && t.Culture == culture).FirstOrDefault();
            if (template == null)
                throw new ArgumentException("No such template " + templateName);

            var package = templateProcessor.CreatePackageFromTemplate(template, model);
            package.To = new RecipientList(to);

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                {
                    package.Attachments.Add(attachment.FullName);
                }
            }

            _packageRelayer.Relay(package);
          
        }

        public void Send(string to, string templateName, Dictionary<string, string> tokenReplacements, string culture = "pl")
        {
            Send(new string[] { to }, templateName, tokenReplacements, null, culture);
        }

        public void Send(string[] to, string templateName, Dictionary<string, string> tokenReplacements, string culture = "en")
        {
            Send(to, templateName, tokenReplacements, null, culture);
        }

        public void Send(string[] to, string templateName,
            Dictionary<string,string> tokenReplacements,
            FileInfo[] fileAttachments, string culture = "pl")
        {
            logger.DebugFormat("sending email {0}.{1}", templateName, culture); ;
            var template = _templateManager.Templates.Where(t => t.Name == templateName && t.Culture == culture).FirstOrDefault();
            if (template ==null)
                throw new ArgumentException("No such template " + templateName);

            NameValueCollection nvc = new NameValueCollection();
            if (tokenReplacements != null)
            {
                foreach (var token in tokenReplacements)
                    nvc.Add(token.Key, token.Value);
            }

            var package = templateProcessor.CreatePackageFromTemplate(template,
                                                                      nvc);
            package.To = new RecipientList(to);

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                {
                    package.Attachments.Add(attachment.FullName);
                }
            }

            _packageRelayer.Relay(package);
        }

        public void LoadTemplates()
        {
            _templateManager.LoadTemplates();
        }
    }
}
