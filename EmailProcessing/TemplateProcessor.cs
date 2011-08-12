using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using log4net;
using RazorEngine.Templating;

namespace EmailProcessing
{
    public interface ITemplateProcessor
    {
        IEmailPackage CreatePackageFromTemplate(EmailTemplate template, NameValueCollection replacements);
        IEmailPackage CreatePackageFromTemplate<T>(EmailTemplate template, T model);
    }

    public class TemplateProcessor : ITemplateProcessor
    {
        private ILog _log = LogManager.GetLogger(typeof (TemplateProcessor));
        public IEmailPackage CreatePackageFromTemplate(EmailTemplate template, NameValueCollection replacements)
        {
            EmailPackage package = new EmailPackage()
                                       {
                                           Subject = template.Subject,
                                           Html = template.Html,
                                           Text = template.Text,
                                           From = template.From,
                                           Identifier = string.Format("{0}-{1}", template.Name, DateTime.Now.Ticks)
                                       };

            foreach(var key in replacements.AllKeys)
            {
                package.Subject = package.Subject.Replace("{"+key+"}", replacements[key]);
                if (!string.IsNullOrWhiteSpace(package.Html)) package.Html = package.Html.Replace("{" + key + "}", replacements[key]);
                if (!string.IsNullOrWhiteSpace(package.Text)) package.Text = package.Text.Replace("{" + key + "}", replacements[key]);
            }

            return package;
        }

        public IEmailPackage CreatePackageFromTemplate<T>(EmailTemplate template, T model)
        {
            EmailPackage package = new EmailPackage()
            {
                Subject = template.Subject,
                Html = template.Html,
                Text = template.Text,
                From = template.From,
                Identifier = string.Format("{0}-{1}", template.Name, DateTime.Now.Ticks)
            };

            try
            {
                package.Html = RazorEngine.Razor.Parse(template.Html, model, null);
            } catch (TemplateCompilationException ex)
            {
                _log.Error("Failed to process template for " + template.Name + " html", ex);
                foreach(var error in ex.Errors)
                {
                    _log.Error(error.ErrorText);
                    _log.Error(error.Line);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to process template for "+template.Name + " html", ex);
            }

            try
            {
                package.Text = RazorEngine.Razor.Parse(template.Text, model, null);
            }
            catch (TemplateCompilationException ex)
            {
                _log.Error("Failed to process template for " + template.Name + " text", ex);
                foreach (var error in ex.Errors)
                {
                    _log.Error(error.ErrorText);
                    _log.Error(error.Line);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to process template for " + template.Name + " text", ex);
            }

            try {
                package.Subject = RazorEngine.Razor.Parse(template.Subject, model, null);
            }
            catch (TemplateCompilationException ex)
            {
                _log.Error("Failed to process template for " + template.Name + " subject", ex);
                foreach (var error in ex.Errors)
                {
                    _log.Error(error.ErrorText);
                    _log.Error(error.Line);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to process template for " + template.Name + " subject", ex);
            }

            return package;
        }
    }
}
