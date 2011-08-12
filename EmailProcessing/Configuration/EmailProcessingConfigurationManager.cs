using System.Configuration;

namespace EmailProcessing.Configuration
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
}