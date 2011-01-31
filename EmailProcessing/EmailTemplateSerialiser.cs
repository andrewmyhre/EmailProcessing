using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EmailProcessing
{
    public class EmailTemplateSerialiser : IEmailTemplateSerialiser
    {
        private const string jgNamespace = "http://www.justgiving.com/xml/";
        public EmailTemplateSerialiser()
        {
        }

        public EmailTemplate Deserialize(string templateContents)
        {
            XDocument xdoc = XDocument.Parse(templateContents, LoadOptions.None);
            return new EmailTemplate()
                {
                    From = xdoc.Element(XName.Get("emailPackage", jgNamespace)).Element(XName.Get("from", jgNamespace)).Value,
                    Subject = xdoc.Element(XName.Get("emailPackage", jgNamespace)).Element(XName.Get("subject", jgNamespace)).Value,
                    Html=xdoc.Element(XName.Get("emailPackage", jgNamespace)).Element(XName.Get("html", jgNamespace)).Value,
                    Text= xdoc.Element(XName.Get("emailPackage", jgNamespace)).Element(XName.Get("text", jgNamespace)).Value,
                    Tokens = new TokenList((from t in xdoc.Element(XName.Get("emailPackage", jgNamespace)).Element(XName.Get("tokens", jgNamespace)).Elements(XName.Get("token", jgNamespace))
                              select t.Value)),
                    Attachments = new AttachmentList(
                        (from a in xdoc
                                    .Element(XName.Get("emailPackage", jgNamespace))
                                    .Element(XName.Get("attachments", jgNamespace))
                                    .Elements(XName.Get("attachment", jgNamespace))
                             select a.Value))
                                
                };
        }

        public string Serialise(EmailTemplate template)
        {
            DataContractSerializer serialiser = new DataContractSerializer(typeof(EmailTemplate));

            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            serialiser.WriteObject(xw, template);
            xw.Flush();
            xw.Close();

            return sb.ToString();
        }
    }
}