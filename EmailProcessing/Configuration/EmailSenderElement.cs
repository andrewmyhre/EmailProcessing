using System.Configuration;

namespace EmailProcessing.Configuration
{
    public class EmailSenderElement:  ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type { get { return (string)this["type"]; } set{this["type"]=value;} }
    }
}