using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace EmailProcessing
{
    public interface ITemplateProcessor
    {
        EmailPackage CreatePackageFromTemplate(EmailTemplate template, NameValueCollection replacements);
    }

    public class TemplateProcessor : ITemplateProcessor
    {
        public EmailPackage CreatePackageFromTemplate(EmailTemplate template, NameValueCollection replacements)
        {
            EmailPackage package = new EmailPackage()
                                       {
                                           Subject = template.Subject,
                                           Html = template.Html,
                                           Text = template.Text,
                                           From = template.From
                                       };

            foreach(var key in replacements.AllKeys)
            {
                package.Subject = package.Subject.Replace("{"+key+"}", replacements[key]);
                if (!string.IsNullOrWhiteSpace(package.Html)) package.Html = package.Html.Replace("{" + key + "}", replacements[key]);
                if (!string.IsNullOrWhiteSpace(package.Text)) package.Text = package.Text.Replace("{" + key + "}", replacements[key]);
            }

            return package;
        }
    }
}
