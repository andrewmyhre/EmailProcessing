using System.Configuration;
using log4net;

namespace EmailProcessing.Configuration
{
    public static class EmailProcessingConfigurationManager
    {
        private static ILog Log = LogManager.GetLogger(typeof (EmailProcessingConfigurationManager));
        public static EmailProcessingConfigurationSection GetConfiguration()
        {
            Log.Debug("loading configuration");
            return ConfigurationManager.GetSection("emailProcessing") as EmailProcessingConfigurationSection;
        }
    }
}