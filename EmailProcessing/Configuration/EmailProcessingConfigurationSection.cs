using System.Configuration;

namespace EmailProcessing.Configuration
{
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
}