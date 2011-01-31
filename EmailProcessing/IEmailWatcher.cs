using System;

namespace EmailProcessing
{
    public interface IEmailWatcher : IDisposable
    {
        event EventHandler<EmailToSendArgs> OnMailToSend;
        void StartWatching();
    }
}