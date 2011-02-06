using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.SimpleEmail.Model;
using log4net;

namespace EmailProcessing
{
    public class AmazonSESEmailSender : EmailSender
    {
        private ILog logger = LogManager.GetLogger(typeof (AmazonSESEmailSender));

        public AmazonSESEmailSender () : base()
        {
            
        }

        public AmazonSESEmailSender(string deliveredLocation, string failedLocation)
            : base(deliveredLocation, failedLocation)
        {
            
        }

        public override void SendMail(object sender, EmailToSendArgs e)
        {
            using (var ses = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(
                EmailProcessingConfigurationManager.Section.Amazon.Key,
                EmailProcessingConfigurationManager.Section.Amazon.Secret))
            {


                Destination destination = new Destination();
                destination.WithToAddresses(e.Message.To);

                Content subject = new Content();
                subject.WithCharset("UTF-8");
                subject.WithData(e.Message.Subject);

                Content html = new Content();
                html.WithCharset("UTF-8");
                html.WithData(e.Message.Html);

                Content text = new Content();
                text.WithCharset("UTF-8");
                text.WithData(e.Message.Text);

                Body body = new Body();
                body.WithHtml(html);
                body.WithText(text);

                Message message = new Message();
                message.WithBody(body);
                message.WithSubject(subject);

                SendEmailRequest request = new SendEmailRequest();
                request.WithDestination(destination);
                request.WithMessage(message);
                request.WithSource(e.Message.From);

                try
                {
                    SendEmailResponse response = ses.SendEmail(request);

                    SendEmailResult result = response.SendEmailResult;

                    logger.Debug("Email sent.");
                    logger.Debug(String.Format("Message ID: {0}",
                                               result.MessageId));
                }
                catch (Exception ex)
                {
                    logger.Fatal("failed to send email", ex);

                    e.SendingFailed = true;
                }
            }
            base.SendMail(sender, e);
        }

        public void VerifyEmail(string email)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(
                EmailProcessingConfigurationManager.Section.Amazon.Key,
                EmailProcessingConfigurationManager.Section.Amazon.Secret))
            {

                client.VerifyEmailAddress(new VerifyEmailAddressRequest() {EmailAddress = email});
            }
        }
    }
}
