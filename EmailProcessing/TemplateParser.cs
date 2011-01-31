using System;
using System.Linq;
using System.Xml.Linq;

namespace EmailProcessing
{
    public class TemplateParser : ITemplateParser
    {
        private const string ns = "emailnamespace";
        public EmailTemplate Parse(string readAllText)
        {
            XDocument xdoc = XDocument.Parse(readAllText);
            
            return new EmailTemplate()
            {
                Name = xdoc.Element(XName.Get("emailTemplate", ns)).Element(XName.Get("name", ns)).Value,
                From = xdoc.Element(XName.Get("emailTemplate", ns)).Element(XName.Get("from", ns)).Value,
                Subject = xdoc.Element(XName.Get("emailTemplate", ns)).Element(XName.Get("subject", ns)).Value,
                Text = xdoc.Element(XName.Get("emailTemplate", ns)).Element(XName.Get("text", ns)).Value,
                Html = xdoc.Element(XName.Get("emailTemplate", ns)).Element(XName.Get("html", ns)).Value,
                Tokens = new TokenList((from t in xdoc.Element(XName.Get("emailTemplate",ns))
                              .Element(XName.Get("tokens",ns))
                              .Elements(XName.Get("token",ns))
                              select t.Value).ToArray())
            };
        }
    }
}