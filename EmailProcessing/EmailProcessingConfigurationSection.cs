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
        public static EmailProcessingConfigurationSection Section
        {
            get
            {
                return ConfigurationManager.GetSection("emailProcessing") as EmailProcessingConfigurationSection;
            }
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
        public string TemplateLocation { get; set; }
    }

    public class EmailSenderElement:  ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type { get { return (string)this["type"]; } set{this["type"]=value;} }
    }
}
