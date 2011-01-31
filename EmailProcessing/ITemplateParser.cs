namespace EmailProcessing
{
    public interface ITemplateParser
    {
        EmailTemplate Parse(string readAllText);
    }
}