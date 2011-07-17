using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EmailProcessing
{
    public class EmailPackageSerialiser : IEmailPackageSerialiser
    {
        public EmailPackage Deserialize(string packageContents)
        {
            XDocument xdoc = XDocument.Parse(packageContents, LoadOptions.None);
            var serialisedPackage = xdoc.Element(XName.Get("emailPackage", Constants.XmlNamespace));
            return new EmailPackage()
                {
                    From = serialisedPackage.Element(XName.Get("from", Constants.XmlNamespace)).Value,
                    Subject = serialisedPackage.Element(XName.Get("subject", Constants.XmlNamespace)).Value,
                    Html= serialisedPackage.Element(XName.Get("html", Constants.XmlNamespace)) != null
                    ? serialisedPackage.Element(XName.Get("html", Constants.XmlNamespace)).Value
                    : null,
                    Text= serialisedPackage.Element(XName.Get("text", Constants.XmlNamespace)) != null
                    ? serialisedPackage.Element(XName.Get("text", Constants.XmlNamespace)).Value
                    : null,
                    To = new RecipientList(from t in serialisedPackage
                                               .Element(XName.Get("recipients", Constants.XmlNamespace))
                                               .Elements(XName.Get("address", Constants.XmlNamespace))
                                            select t.Value),
                    Attachments = new AttachmentList(
                        (from a in serialisedPackage
                                    .Element(XName.Get("attachments", Constants.XmlNamespace))
                                    .Elements(XName.Get("attachment", Constants.XmlNamespace))
                             select a.Value))
                                
                };
        }

        public string Serialise(EmailPackage package)
        {
            //DataContractSerializer serialiser = new DataContractSerializer(typeof(EmailPackage));
            XmlSerializer serialiser = new XmlSerializer(typeof(EmailPackage));

            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            serialiser.Serialize(xw, package);
            xw.Flush();
            xw.Close();

            return sb.ToString();
        }
    }
}