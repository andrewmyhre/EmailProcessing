using System;

namespace EmailProcessing
{
    public static class EmailSenderFactory
    {
        public static IEmailSender CreateSenderFromConfiguration()
        {
            Type senderType = Type.GetType(EmailProcessingConfigurationManager.Section.EmailSenderType.Type);
            IEmailSender sender = Activator.CreateInstance(senderType, null) as IEmailSender;
            return sender;
        }
    }
}