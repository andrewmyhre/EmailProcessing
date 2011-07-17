using System.Collections.Generic;
using System.IO;

namespace EmailProcessing
{
    public interface IEmailFacade
    {
        void Send<T>(string to, string templateName, T model);

        void Send<T>(string[] to, string templateName,
                     T model);

        void Send<T>(string[] to, string templateName,
                     T model,
                     FileInfo[] fileAttachments);

        void Send(string to, string templateName, Dictionary<string, string> tokenReplacements);
        void Send(string[] to, string templateName, Dictionary<string, string> tokenReplacements);

        void Send(string[] to, string templateName,
                  Dictionary<string,string> tokenReplacements,
                  FileInfo[] fileAttachments);

        void LoadTemplates();
    }
}