using System;
using EmailProcessing.Configuration;

namespace EmailProcessing
{
    public static class EmailSenderFactory
    {
        public static IEmailSender CreateSenderFromConfiguration(EmailProcessingConfigurationSection configuration)
        {
            Type senderType = Type.GetType(configuration.EmailSenderType.Type);
            IEmailSender sender = Activator.CreateInstance(senderType, configuration) as IEmailSender;
            return sender;
        }

        public static IEmailPackageRelayer CreateRelayerFromConfiguration(EmailBuilderConfigurationSection configuration)
        {
            Type relayerType = Type.GetType(configuration.EmailSenderType.Type);
            IEmailPackageRelayer relayer = Activator.CreateInstance(relayerType, configuration) as IEmailPackageRelayer;
            return relayer;
        }
    }
}