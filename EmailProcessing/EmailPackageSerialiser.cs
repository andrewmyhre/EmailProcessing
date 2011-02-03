using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EmailProcessing
{
    public class EmailPackageSerialiser : IEmailPackageSerialiser
    {
        private const string jgNamespace = "http://www.tanash.net/email/package";
        public EmailPackageSerialiser()
        {
        }

        public EmailPackage Deserialize(string packageContents)
        {
            XDocument xdoc = XDocument.Parse(packageContents, LoadOptions.None);
            var serialisedPackage = xdoc.Element(XName.Get("emailPackage", jgNamespace));
            return new EmailPackage()
                {
                    From = serialisedPackage.Element(XName.Get("from", jgNamespace)).Value,
                    Subject = serialisedPackage.Element(XName.Get("subject", jgNamespace)).Value,
                    Html=serialisedPackage.Element(XName.Get("html", jgNamespace)).Value,
                    Text= serialisedPackage.Element(XName.Get("text", jgNamespace)).Value,
                    To = new RecipientList(from t in serialisedPackage
                                               .Element(XName.Get("recipients", jgNamespace))
                                               .Elements(XName.Get("address", jgNamespace))
                                            select t.Value),
                    Attachments = new AttachmentList(
                        (from a in serialisedPackage
                                    .Element(XName.Get("attachments", jgNamespace))
                                    .Elements(XName.Get("attachment", jgNamespace))
                             select a.Value))
                                
                };
        }

        public string Serialise(EmailPackage package)
        {
            DataContractSerializer serialiser = new DataContractSerializer(typeof(EmailPackage));

            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            serialiser.WriteObject(xw, package);
            xw.Flush();
            xw.Close();

            return sb.ToString();
        }
    }
}