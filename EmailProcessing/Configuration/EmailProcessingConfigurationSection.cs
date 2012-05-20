using System;
using System.Configuration;

namespace EmailProcessing.Configuration
{
    public class EmailProcessingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("senderType", IsRequired = true)]
        public string EmailSenderType { get { return (string)this["senderType"]; } }
        [ConfigurationProperty("pickupLocation", IsRequired = true)]
        public string PickupLocation { get { return (string)this["pickupLocation"]; } set { this["pickupLocation"] = value; } }
        [ConfigurationProperty("failedLocation", IsRequired = true)]
        public string FailedLocation { get { return (string)this["failedLocation"]; } set { this["failedLocation"] = value; } }
        [ConfigurationProperty("deliveredLocation", IsRequired = true)]
        public string DeliveredLocation { get { return (string)this["deliveredLocation"]; } set { this["deliveredLocation"] = value; } }
        [ConfigurationProperty("amazon")]
        public AmazonElement Amazon { get { return (AmazonElement) this["amazon"]; } }
    }
}