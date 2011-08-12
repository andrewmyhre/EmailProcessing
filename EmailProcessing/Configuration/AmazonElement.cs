using System.Configuration;

namespace EmailProcessing.Configuration
{
    public class AmazonElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key { get { return (string)this["key"]; } set { this["key"] = value; } }
        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret { get { return (string)this["secret"]; } set { this["secret"] = value; } }
    }
}
