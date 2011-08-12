﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing.Configuration;

namespace EmailProcessing
{
    public class EmailFacade : IEmailFacade
    {
        private readonly string _templateLocation;
        private readonly string _pickupLocation;
        private IEmailTemplateManager templateManager = null;
        private ITemplateProcessor templateProcessor = null;
        private IEmailPackageSerialiser _packageSerialiser = null;
        private IEmailPackageRelayer _packageRelayer = null;

        public EmailFacade(EmailBuilderConfigurationSection configuration)
            : this(configuration.EmailSenderType.TemplateLocation, configuration.PickupLocation,
            Activator.CreateInstance(Type.GetType(configuration.EmailSenderType.Type), configuration) as IEmailPackageRelayer)
        {
            
        }

        public EmailFacade(string templateLocation, string pickupLocation, IEmailPackageRelayer packageRelayer)
        {
            _templateLocation = templateLocation;
            _pickupLocation = pickupLocation;
            _packageRelayer = packageRelayer;

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

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                {
                    package.Attachments.Add(attachment.FullName);
                }
            }

            _packageRelayer.Relay(package);
          
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
            templateManager.LoadTemplates();
        }
    }
}
