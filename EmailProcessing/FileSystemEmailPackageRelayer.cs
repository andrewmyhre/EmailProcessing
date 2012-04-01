using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmailProcessing.Configuration;

namespace EmailProcessing
{
    public abstract class EmailPackageRelayer : IEmailPackageRelayer
    {
        protected readonly string OutputLocation;
        protected IEmailPackageSerialiser PackageSerialiser = null;

        public EmailPackageRelayer(EmailBuilderConfigurationSection configuration) : this(configuration.PickupLocation)
        {
        }

        protected EmailPackageRelayer(string outputLocation)
        {
            OutputLocation = outputLocation;
            PackageSerialiser = new EmailPackageSerialiser();
        }

        public abstract void Relay(IEmailPackage package);

        protected IEmailPackage RemoveDuplicateRecipients(IEmailPackage package)
        {
            return package; // todo: implement  
        }
    }

    public interface IEmailPackageRelayer
    {
        void Relay(IEmailPackage package);
    }

    public class FileSystemEmailPackageRelayer : EmailPackageRelayer
    {

        public FileSystemEmailPackageRelayer(EmailBuilderConfigurationSection configuration)
            : base(configuration)
        {
            
        }

        public FileSystemEmailPackageRelayer(string outputLocation)
            : base(outputLocation)
        {
            
        }

        public override void Relay(IEmailPackage package)
        {
            if (string.IsNullOrEmpty(package.Identifier))
            {
                package.Identifier = string.Format("{0}-{1:yyyyMMddhhmmss}", package.Subject.Replace(' ', '-'), DateTime.Now);
            }

            package = base.RemoveDuplicateRecipients(package);

            var xml = PackageSerialiser.Serialise(package);
            string packagePath = Path.Combine(OutputLocation.MapVirtual(), package.Identifier + ".xml");
            File.WriteAllText(packagePath, xml);

        }
    }
}
