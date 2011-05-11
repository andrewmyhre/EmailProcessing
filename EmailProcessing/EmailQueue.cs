using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class EmailQueue : Dictionary<Guid, IEmailPackage>
    {
    }
}
