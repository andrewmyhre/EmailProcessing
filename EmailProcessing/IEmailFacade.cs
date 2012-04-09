using System.Collections.Generic;
using System.IO;

namespace EmailProcessing
{
    public interface IEmailFacade
    {
        void Send<T>(string to, string templateName, T model, string culture = "pl");
        void Send<T>(string[] to, string templateName, T model, FileInfo[] fileAttachments, string culture = "pl");

        void Send(string to, string templateName, Dictionary<string, string> tokenReplacements, string culture = "pl");
        void Send(string[] to, string templateName, Dictionary<string, string> tokenReplacements, string culture = "pl");
        void Send(string[] to, string templateName, Dictionary<string,string> tokenReplacements, FileInfo[] fileAttachments, string culture = "en");

        void LoadTemplates();
    }
}