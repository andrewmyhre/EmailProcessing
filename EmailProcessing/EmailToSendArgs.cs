using System;

namespace EmailProcessing
{
    public class EmailToSendArgs : EventArgs
    {
        public EmailPackage Message { get; set; }

        public string PackagePath { get; set; }

        public bool SendingFailed { get; set; }
    }
}