using System;
using System.Linq;
using System.Xml.Linq;

namespace EmailProcessing
{
    public class TemplateParser : ITemplateParser
    {
        private const string ns = "http://www.tanash.net/email/template";
        public EmailTemplate Parse(string readAllText)
        {
            XDocument xdoc = XDocument.Parse(readAllText);

                var xml = xdoc.Element(XName.Get("emailTemplate", ns));
                var template = new EmailTemplate()
                                   {
                                       Name = xml.Element(XName.Get("name", ns)).Value,
                                       From = xml.Element(XName.Get("from", ns)).Value,
                                       Subject = xml.Element(XName.Get("subject", ns)).Value,
                                       Text = xml.Element(XName.Get("text", ns)) != null
                                       ? xml.Element(XName.Get("text", ns)).Value
                                       : null,
                                       Html = xml.Element(XName.Get("html", ns)) != null
                                       ? xml.Element(XName.Get("html", ns)).Value
                                       : null,
                                       Culture = xml.Element(XName.Get("culture", ns)) != null
                                       ? xml.Element(XName.Get("culture", ns)).Value
                                       : null
                                   };
                return template;
        }
    }
}