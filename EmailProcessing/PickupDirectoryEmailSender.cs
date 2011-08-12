using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmailProcessing.Configuration;

namespace EmailProcessing
{
    public class PickupDirectoryEmailSender : EmailSender
    {

        public PickupDirectoryEmailSender(EmailProcessingConfigurationSection configuration)
            : base(configuration)
        {
            
        }

        public PickupDirectoryEmailSender(string deliveredLocation, string failedLocation) : base(deliveredLocation, failedLocation)
        {
            
        }
    }
}
