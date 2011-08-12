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
    }
}