using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Amazon.SimpleEmail.Model;
using EmailProcessing;
using EmailProcessing.Configuration;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    [Ignore]
    public class AmazonServiceTests
    {
        private EmailProcessingConfigurationSection _configuration;

        [TestCase("andrew.myhre@gmail.com")]
        public void VerifyEmail(string email)
        {
            _configuration = ConfigurationManager.GetSection("emailProcessing") as EmailProcessingConfigurationSection;
            AmazonSESEmailSender aSes = new AmazonSESEmailSender(_configuration);
            aSes.VerifyEmail(email);
        }

        [Test]
        public void SendEmail()
        {
            var client = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(_configuration.Amazon.Key, _configuration.Amazon.Secret);

            Destination destination = new Destination();
            destination.WithToAddresses("andrew.myhre@gmail.com");

            Content subject = new Content();
            subject.WithCharset("UTF-8");
            subject.WithData("subject");

            Content html = new Content();
            html.WithCharset("UTF-8");
            html.WithData(@"<p>Hi {DonorName},</p>
        <p>Thank you for making a donation to {CharityName}. In return for your goodwill we have created your very own papal indulgence. You are free to download it, print it and lord your piousness over your friends!</p>
        <p><a href=""{ServerAuthority}/content/{IndulgenceId}/indulgence.pdf""><img src=""/content/indulgences/{IndulgenceId}/indulgence_25.png"" /></a></p>
        <p><a href=""{ServerAuthority}/content/{IndulgenceId}/indulgence.pdf"">Click here to download your indulgence</a></p>
        <p>Regards,</p>
        <p>Andrew</p>
        <p>IndulgeMe.cc</p>");

            Content text = new Content();
            text.WithCharset("UTF-8");
            text.WithData("text");

            Body body = new Body();
            body.WithHtml(html);
            body.WithText(text);

            Message message = new Message();
            message.WithBody(body);
            message.WithSubject(subject);

            SendEmailRequest request = new SendEmailRequest();
            request.WithDestination(destination);
            request.WithMessage(message);
            request.WithSource("andrew.myhre@gmail.com");

            client.SendEmail(request);
        }

        [Test]
        public void SendEmailWithAttachment()
        {
            var client = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(_configuration.Amazon.Key, _configuration.Amazon.Secret);

            MailMessage m = new MailMessage();
            var attachment = new System.Net.Mail.Attachment("TextFile1.txt", "text/plain");
            attachment.TransferEncoding = TransferEncoding.QuotedPrintable;
            m.Attachments.Add(attachment);
            m.Body = "hello";
            m.Subject = "hello";
            m.To.Add("andrew.myhre@gmail.com");
            m.From = new MailAddress("andrew.myhre@gmail.com");

            var messageData = ConvertMailMessageToMemoryStream(m);
            RawMessage message = new RawMessage(messageData);
            SendRawEmailRequest request = new SendRawEmailRequest(message);
            var response = client.SendRawEmail(request);
        }

        [Test]
        public void Sendraw()
        {
            string raw =
                @"X-Sender: andrew.myhre@gmail.com
X-Receiver: andrew.myhre@gmail.com
MIME-Version: 1.0
From: andrew.myhre@gmail.com
To: andrew.myhre@gmail.com
Date: 9 Feb 2011 17:28:54 +0000
Subject: hello
Content-Type: multipart/mixed; boundary=--boundary_0_6c21630e-f7ee-4968-97de-1940cd84094f


----boundary_0_6c21630e-f7ee-4968-97de-1940cd84094f
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: quoted-printable

hello
----boundary_0_6c21630e-f7ee-4968-97de-1940cd84094f
Content-Type: text/html; name=TextFile1.txt
Content-Transfer-Encoding: quoted-printable
Content-Disposition: attachment

=EF=BB=BFtext file
----boundary_0_6c21630e-f7ee-4968-97de-1940cd84094f--
";
            var client = Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(_configuration.Amazon.Key, _configuration.Amazon.Secret);
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(raw);
            sw.Flush();
            sw.Close();

            var rawMessage = new RawMessage(ms);
            client.SendRawEmail(new SendRawEmailRequest(rawMessage));

        }

        public static MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
        {
            Assembly assembly = typeof(SmtpClient).Assembly;

            Type mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            MemoryStream fileStream = new MemoryStream();

            ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

            object mailWriter = mailWriterContructor.Invoke(new object[] { fileStream });

            MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

            sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true }, null);

            byte[] data = new byte[fileStream.Length];
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(data, 0, (int)fileStream.Length);
            string messageSauce = ASCIIEncoding.ASCII.GetString(data);

            Debug.WriteLine(messageSauce);

            MethodInfo closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

            closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

            return fileStream;
        }
    }
}
