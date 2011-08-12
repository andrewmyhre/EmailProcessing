namespace EmailProcessing
{
    public interface IEmailPackageSerialiser
    {
        EmailPackage Deserialize(string packageContents);
        string Serialise(IEmailPackage package);
    }
}