using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using log4net;

namespace EmailProcessing
{
    public static class LinqToXmlExtensions
    {
        public static string ValueOrEmptyString(this XElement element, string elementName)
        {
            XElement xElement = element.Element(elementName);
            if (xElement != null)
            {
                return xElement.Value;
            }

            return "";
        }
        public static string ValueOrEmptyString(this XElement element, string elementName, string elementNamespace)
        {
            XElement xElement = element.Element(XName.Get(elementName, elementNamespace));
            if (xElement != null)
            {
                return xElement.Value;
            }

            return "";
        }
    }

    public class EmailPackageSerialiser : IEmailPackageSerialiser
    {
        public EmailPackage Deserialize(string packageContents)
        {
            XDocument xdoc = XDocument.Parse(packageContents, LoadOptions.None);
            var serialisedPackage = xdoc.Element(XName.Get("emailPackage", Constants.XmlNamespace));

            if (serialisedPackage == null)
            {
                throw new InvalidOperationException("Could not find emailPackageElement in package contents");
            }

            var recipientsElement = serialisedPackage.Element(XName.Get("recipients", Constants.XmlNamespace));
            var attachmentsElement = serialisedPackage.Element(XName.Get("attachments", Constants.XmlNamespace));

            if (recipientsElement == null || !recipientsElement.HasElements)
            {
                throw new InvalidOperationException("No recipients specified for email");
            }

            var package = new EmailPackage();
            package.From = serialisedPackage.ValueOrEmptyString("from", Constants.XmlNamespace);
            package.Subject = serialisedPackage.ValueOrEmptyString("subject", Constants.XmlNamespace);
            package.Html = serialisedPackage.ValueOrEmptyString("html", Constants.XmlNamespace);
            package.Text = serialisedPackage.ValueOrEmptyString("text", Constants.XmlNamespace);
            package.To = new RecipientList(from t in recipientsElement
                                               .Elements(XName.Get("recipient", Constants.XmlNamespace))
                                           select t.Value);
            if (attachmentsElement != null)
            {
                package.Attachments = new AttachmentList(from a in attachmentsElement.Elements(XName.Get("attachment", Constants.XmlNamespace))
                                           select a.Value);
            }

            return package;
        }

        public string Serialise(IEmailPackage package)
        {
            //DataContractSerializer serialiser = new DataContractSerializer(typeof(EmailPackage));
            XmlSerializer serialiser = new XmlSerializer(typeof(EmailPackage));

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings=new XmlWriterSettings()
                                           {
                                               Indent = true,
                                               NewLineHandling = NewLineHandling.Entitize,
                                               NewLineOnAttributes = true
                                           };
            XmlWriter xw = XmlWriter.Create(sb, settings);
            serialiser.Serialize(xw, package);
            xw.Flush();
            xw.Close();

            return sb.ToString();
        }
    }
}