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
            var xml = PackageSerialiser.Serialise(package);
            string packagePath = Path.Combine(OutputLocation, package.Identifier + ".xml");
            File.WriteAllText(packagePath, xml);

        }
    }
}
