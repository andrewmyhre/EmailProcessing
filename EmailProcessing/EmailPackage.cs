using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace EmailProcessing
{
    [DataContract(Name = "emailPackage", Namespace = Constants.XmlNamespace)]
    public class EmailPackage : IEmailPackage
    {
        public EmailPackage()
        {
            Attachments = new AttachmentList();
            To = new RecipientList();
        }

        [DataMember(Name="from")]
        public string From { get; set; }
        [DataMember(Name = "subject")]
        public string Subject { get; set;}
        [DataMember(Name = "html")]
        public string Html { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "attachments")]
        public AttachmentList Attachments { get; set; }
        [DataMember(Name="recipients")]
        public RecipientList To { get; set; }
        [IgnoreDataMember]
        public string PackageLocation { get; set; }
    }

    [CollectionDataContract(Name = "attachments", ItemName = "attachment", Namespace = Constants.XmlNamespace)]
    public class AttachmentList : Collection<string>
    {
        public AttachmentList() : base()
        {
            
        }
        public AttachmentList(IEnumerable<string> attachments)
        {
            foreach(string a in attachments)
                Add(a);
        }
    }

    [CollectionDataContract(Name = "recipients", ItemName = "address", Namespace = Constants.XmlNamespace)]
    public class RecipientList : Collection<string>
    {
        public RecipientList() : base()
        {
            
        }
        public RecipientList(IEnumerable<string> recipients)
        {
            foreach(string r in recipients)
                Add(r);
        }
    }

}