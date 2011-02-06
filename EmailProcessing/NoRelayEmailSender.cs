using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class NoRelayEmailSender : EmailSender
    {

        public NoRelayEmailSender () : base()
        {
            
        }

        public NoRelayEmailSender(string deliveredLocation, string failedLocation) : base(deliveredLocation, failedLocation)
        {
            
        }
    }
}
