using System.Runtime.Serialization;

namespace EmailProcessing
{
    public interface IEmailPackage
    {
        [DataMember(Name = "from")]
        string From { get; set; }

        [DataMember(Name = "subject")]
        string Subject { get; set; }

        [DataMember(Name = "html")]
        string Html { get; set; }

        [DataMember(Name = "text")]
        string Text { get; set; }

        [DataMember(Name = "attachments")]
        AttachmentList Attachments { get; set; }

        [DataMember(Name = "recipients")]
        RecipientList To { get; set; }
    }
}