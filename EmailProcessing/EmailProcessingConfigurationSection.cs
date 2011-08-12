using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public static class EmailProcessingConfigurationManager
    {
        public static EmailProcessingConfigurationSection GetConfiguration()
        {
            var configMap = new ExeConfigurationFileMap() { ExeConfigFilename = "EmailProcessing.config"};
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            return config.GetSection("emailProcessing") as EmailProcessingConfigurationSection;
        }
    }

    public class EmailProcessingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("emailSender", IsRequired = true)]
        public EmailSenderElement EmailSenderType { get { return (EmailSenderElement)this["emailSender"]; } set { this["emailSender"] = value; } }
        [ConfigurationProperty("pickupLocation", IsRequired = true)]
        public string PickupLocation { get { return (string)this["pickupLocation"]; } set { this["pickupLocation"] = value; } }
        [ConfigurationProperty("failedLocation", IsRequired = true)]
        public string FailedLocation { get { return (string)this["failedLocation"]; } set { this["failedLocation"] = value; } }
        [ConfigurationProperty("deliveredLocation", IsRequired = true)]
        public string DeliveredLocation { get { return (string)this["deliveredLocation"]; } set { this["deliveredLocation"] = value; } }
        [ConfigurationProperty("templateLocation", IsRequired = true)]
        public string TemplateLocation { get { return (string)this["templateLocation"]; } set { this["templateLocation"]=value; } }
        [ConfigurationProperty("amazon")]
        public AmazonElement Amazon { get { return (AmazonElement) this["amazon"]; } }
    }

    public class EmailSenderElement:  ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type { get { return (string)this["type"]; } set{this["type"]=value;} }
    }

    public class AmazonElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key { get { return (string)this["key"]; } set { this["key"] = value; } }
        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret { get { return (string)this["secret"]; } set { this["secret"] = value; } }
    }
}
