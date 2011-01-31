using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace EmailProcessing
{
    [DataContract(Name = "emailPackage", Namespace = "http://www.justgiving.com/xml/")]
    public class EmailPackage
    {
        public EmailPackage()
        {
            Attachments = new AttachmentList();
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
    }

    [CollectionDataContract(Name="attachments",ItemName="attachment",Namespace="http://www.justgiving.com/xml/"  )]
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


}