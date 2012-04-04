using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace EmailProcessing
{
    [DataContract(Name = "emailPackage", Namespace = "http://www.tanash.net/email/package")]
    public class EmailTemplate
    {
        [DataMember(Name="from")]
        public string From { get; set; }
        [DataMember(Name = "subject")]
        public string Subject { get; set;}
        [DataMember(Name = "html")]
        public string Html { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name="tokens")]
        public TokenList Tokens { get; set; }
        [DataMember(Name="templateName")]
        public string Name { get; set; }
        [DataMember(Name = "culture")]
        public string Culture { get; set; }
    }

    [CollectionDataContract(Name = "tokens", ItemName = "token", Namespace = "http://www.tanash.net/email/package")]
    public class TokenList : Collection<string>
    {
        public TokenList() : base()
        {
            
        }
        public TokenList(IEnumerable<string> tokens)
        {
            foreach(string t in tokens)
                Add(t);
        }
    }
}