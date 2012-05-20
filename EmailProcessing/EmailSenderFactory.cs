using System;
using EmailProcessing.Configuration;

namespace EmailProcessing
{
    public static class EmailSenderFactory
    {
        public static IEmailSender CreateSenderFromConfiguration(EmailProcessingConfigurationSection configuration)
        {
            IEmailSender sender = Activator.CreateInstance(Type.GetType(configuration.EmailSenderType), configuration) as IEmailSender;
            return sender;
        }

        public static IEmailPackageRelayer CreateRelayerFromConfiguration(EmailBuilderConfigurationSection configuration)
        {
            IEmailPackageRelayer relayer = Activator.CreateInstance(Type.GetType(configuration.EmailSenderType), configuration) as IEmailPackageRelayer;
            return relayer;
        }
    }
}