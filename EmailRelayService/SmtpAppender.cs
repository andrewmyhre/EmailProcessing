using System;
using System.IO;
using log4net.Appender;
using log4net.Core;
using System.Net.Mail;
using System.Net;

namespace EmailRelayService
{
    /// <summary>
    /// The standard log4net SmtpAppender doesn't support SSL authentication, which is
    /// required to send email via gmail.
    ///
    /// This appender uses the SmtpClient (only available in .NET 2.0) to send SMTP mail that
    /// is secured via SSL.  This is needed to talk to the gmail SMTP server. 
    ///
    /// This code is heavily based on that posted by Ron Grabowski at:
    /// http://mail-archives.apache.org/mod_mbox/logging-log4net-user/200602.mbox/%3C20060216123155.22007.qmail@web32202.mail.mud.yahoo.com%3E
    /// </summary>
    public class SmtpClientSmtpAppender : SmtpAppender
    {
        override protected void SendBuffer(LoggingEvent[] events)
        {
            try
            {
                StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                string t = Layout.Header;
                if (t != null)
                {
                    writer.Write(t);
                }
                for (int i = 0; i < events.Length; i++)
                {
                    // Render the event and append the text to the buffer
                    RenderLoggingEvent(writer, events[i]);
                }
                t = Layout.Footer;
                if (t != null)
                {
                    writer.Write(t);
                }
                // Use SmtpClient so we can use SSL.
                SmtpClient client = new SmtpClient(SmtpHost, Port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(Username, Password);
                string messageText = writer.ToString();
                MailMessage mail = new MailMessage(From, To, Subject, messageText);
                client.Send(mail);
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error occurred while sending e-mail notification from SmtpClientSmtpAppender.", e);
            }
        }
    }
}