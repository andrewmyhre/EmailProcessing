﻿using System.Collections.Generic;
using System.IO;

namespace EmailProcessing
{
    public interface IEmailTemplateManager
    {
        IEnumerable<EmailTemplate> Templates { get; }
        void LoadTemplates();
    }

    public class EmailTemplateManager : IEmailTemplateManager
    {
        private readonly string _templateFolder;
        private readonly ITemplateParser _templateParser = null;
        private readonly List<EmailTemplate> _templates = new List<EmailTemplate>();
        public IEnumerable<EmailTemplate> Templates { get { return _templates; } }

        public EmailTemplateManager(string templateFolder)
        {
            _templateFolder = templateFolder;
            _templateParser = new TemplateParser();
        }

        public void LoadTemplates()
        {
            DirectoryInfo dir = new DirectoryInfo(_templateFolder);

            var files = dir.GetFiles("*.xml");
            foreach(var file in files)
            {
                _templates.Add(_templateParser.Parse(File.ReadAllText(file.FullName)));
            }
        }
    }
}