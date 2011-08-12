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
        private readonly EmailProcessingConfigurationSection _configuration;
        private ILog logger = LogManager.GetLogger(typeof (AmazonSESEmailSender));
        private static DateTime _lastSend = DateTime.MinValue;
        private static DateTime nextCanSend = DateTime.Now;
        private double _maxSendRate;

        public AmazonSESEmailSender(EmailProcessingConfigurationSection configuration)
            : base(configuration)
        {
            _configuration = configuration;
            using (var ses = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(
                configuration.Amazon.Key,
                configuration.Amazon.Secret))
            {
                var response = ses.GetSendQuota(new GetSendQuotaRequest());
                _maxSendRate = response.GetSendQuotaResult.MaxSendRate;
                logger.InfoFormat("Amazon SES max send rate is {0:#.#} emails per second", _maxSendRate);
            }
        }

        public AmazonSESEmailSender(string deliveredLocation, string failedLocation)
            : base(deliveredLocation, failedLocation)
        {
            
        }

        public override void SendMail(object sender, EmailToSendArgs e)
        {
            if (nextCanSend > DateTime.Now)
                System.Threading.Thread.Sleep(nextCanSend.Subtract(DateTime.Now));

            using (var ses = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(
                _configuration.Amazon.Key,
                _configuration.Amazon.Secret))
            {
                Destination destination = new Destination();
                destination.WithToAddresses(e.Message.To);

                Content subject = new Content();
                subject.WithCharset("UTF-8");
                subject.WithData(e.Message.Subject);

                Body body = new Body();
                if (!string.IsNullOrWhiteSpace(e.Message.Html))
                {
                    Content html = new Content();
                    html.WithCharset("UTF-8");
                    html.WithData(e.Message.Html);
                    body.WithHtml(html);
                }

                if (!string.IsNullOrWhiteSpace(e.Message.Text))
                {
                    Content text = new Content();
                    text.WithCharset("UTF-8");
                    text.WithData(e.Message.Text);
                    body.WithText(text);
                }

                Message message = new Message();
                message.WithBody(body);
                message.WithSubject(subject);

                SendEmailRequest request = new SendEmailRequest();
                request.WithDestination(destination);
                request.WithMessage(message);
                request.WithSource(e.Message.From);

                try
                {
                    Console.WriteLine("sending email from {0}", e.Message.From);
                    SendEmailResponse response = ses.SendEmail(request);

                    SendEmailResult result = response.SendEmailResult;

                    logger.Debug("Email sent.");
                    logger.Debug(String.Format("Message ID: {0}",
                                               result.MessageId));

                    nextCanSend = _lastSend.AddSeconds(_maxSendRate);
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
                _configuration.Amazon.Key,
                _configuration.Amazon.Secret))
            {

                client.VerifyEmailAddress(new VerifyEmailAddressRequest() {EmailAddress = email});
            }
        }
    }
}
