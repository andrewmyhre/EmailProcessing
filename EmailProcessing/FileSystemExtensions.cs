using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public static class FileSystemExtensions
    {
        public static string MapVirtual(this string path)
        {
            string mapped=path;
            if (mapped.StartsWith("~"))
            {
                mapped = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(2));
            }


            return mapped;
        }
    }
}
