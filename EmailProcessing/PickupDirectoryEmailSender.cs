using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class PickupDirectoryEmailSender : EmailSender
    {

        public PickupDirectoryEmailSender () : base()
        {
            
        }

        public PickupDirectoryEmailSender(string deliveredLocation, string failedLocation) : base(deliveredLocation, failedLocation)
        {
            
        }
    }
}
