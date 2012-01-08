using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EmailProcessing
{
    [DataContract(Name = "emailPackage", Namespace = Constants.XmlNamespace)]
    [System.Xml.Serialization.XmlRoot(ElementName="emailPackage", Namespace=Constants.XmlNamespace)]
    public class EmailPackage : IEmailPackage
    {
        public EmailPackage()
        {
            Attachments = new AttachmentList();
            To = new RecipientList();
        }

        [DataMember(Name="from")]
        [XmlElement("from")]
        public string From { get; set; }
        [DataMember(Name = "subject")]
        [XmlElement("subject")]
        public string Subject { get; set;}
        [XmlIgnore]
        public string Html { get; set; }
        [XmlElement("html")]
        [DataMember(Name = "html")]
        public XmlCDataSection HtmlCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(Html);
            }
            set { Html = value.Value; }
        }
        [DataMember(Name = "text")]
        [XmlElement("text")]
        public string Text { get; set; }
        [DataMember(Name = "attachments")]
        [XmlArray("attachments")]
        [XmlArrayItem("attachments", ElementName="attachment")]
        public AttachmentList Attachments { get; set; }
        [XmlArray("recipients")]
        [XmlArrayItem("recipients", ElementName="recipient")]
        public RecipientList To { get; set; }
        [IgnoreDataMember]
        [XmlIgnore]
        public string PackageLocation { get; set; }
        [DataMember(Name="identifier")]
        [XmlElement("identifier")]
        public string Identifier { get; set; }
    }

    [XmlRoot("attachment", Namespace = Constants.XmlNamespace)]
    public class Attachment
    {
        public Attachment()
        {
            
        }
        public Attachment(string path)
        {
            Path = path;
        }

        [XmlElement("path")]
        public string Path { get; set; }
    }

    [CollectionDataContract(Name = "attachments", ItemName = "attachment", KeyName = "attachment", Namespace = Constants.XmlNamespace)]
    public class AttachmentList : List<Attachment>
    {
        public AttachmentList() : base()
        {
            
        }
        public AttachmentList(IEnumerable<string> paths )
        {
            AddRange(paths.Select(p=>new Attachment(p)));
        }
        public AttachmentList(IEnumerable<Attachment> attachments)
        {
            foreach (Attachment a in attachments)
                Add(a);
        }

        public void Add(string path)
        {
            Add(new Attachment(path));
        }
    }

    [XmlRoot("recipient", Namespace = Constants.XmlNamespace)]
    public class Recipient
    {
        public Recipient()
        {
            
        }
        public Recipient(string recipient)
        {
            // todo: check for name <address> format recipients
            EmailAddress = recipient;
        }
        public Recipient(string name, string address)
        {
            Name = name;
            EmailAddress = address;
        }

        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("emailAddress")]
        public string EmailAddress { get; set; }
    }

    [CollectionDataContract(Name = "recipients", ItemName = "recipient", KeyName = "attachment", Namespace = Constants.XmlNamespace)]
    public class RecipientList : List<Recipient>
    {
        public RecipientList() : base()
        {
            
        }
        public RecipientList(IEnumerable<string> recipients )
        {
            AddRange(recipients.Select(r => new Recipient(r)));
        }
        public RecipientList(IEnumerable<Recipient> recipients)
        {
            foreach(var r in recipients)
                Add(r);
        }

        public void Add(string recipient)
        {
            Add(new Recipient(recipient));
        }

        public string[] ToStringArray()
        {
            return (from r in this
                    where !string.IsNullOrWhiteSpace(r.Name)
                    select string.Format("{0} <{1}>", r.Name, r.EmailAddress))
                    .Union((from r in this
                                where string.IsNullOrWhiteSpace(r.Name)
                                select r.EmailAddress))
                .ToArray();
        }
    }

}