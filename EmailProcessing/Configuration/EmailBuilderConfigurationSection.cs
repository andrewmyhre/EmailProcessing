using System.Configuration;

namespace EmailProcessing.Configuration
{
    public class EmailBuilderConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pickupLocation", IsRequired = true)]
        public string PickupLocation { get { return (string)this["pickupLocation"]; } set { this["pickupLocation"] = value; } }
        [ConfigurationProperty("emailBuilder", IsRequired = true)]
        public EmailSenderElement EmailSenderType { get { return (EmailSenderElement)this["emailBuilder"]; } set { this["emailBuilder"] = value; } }
    }
}