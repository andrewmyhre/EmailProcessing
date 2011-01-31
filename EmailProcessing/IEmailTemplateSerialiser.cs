namespace EmailProcessing
{
    public interface IEmailTemplateSerialiser
    {
        EmailTemplate Deserialize(string templateContents);
    }
}