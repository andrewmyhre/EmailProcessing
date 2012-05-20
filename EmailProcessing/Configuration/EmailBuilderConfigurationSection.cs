using System;
using System.Configuration;

namespace EmailProcessing.Configuration
{
    public class EmailBuilderConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pickupLocation", IsRequired = true)]
        public string PickupLocation { get { return (string)this["pickupLocation"]; } set { this["pickupLocation"] = value; } }
        [ConfigurationProperty("senderType", IsRequired = true)]
        public string EmailSenderType { get { return (string)this["senderType"]; } }
        [ConfigurationProperty("templateLocation", IsRequired=true)]
        public string TemplateLocation { get { return (string)this["templateLocation"];}}
    }
}