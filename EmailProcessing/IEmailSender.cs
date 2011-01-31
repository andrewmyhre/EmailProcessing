namespace EmailProcessing
{
    public interface IEmailSender
    {
        void SendMail(object sender, EmailToSendArgs e);
    }
}