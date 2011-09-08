using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailProcessing
{
    public class Util
    {
        public static string DevirtualizePath(string path)
        {
            if (path.StartsWith("~/"))
            {
                if (System.Web.HttpContext.Current == null)
                {
                    throw new ArgumentException("Cannot use a virtual path for template location when not running in the context of a web application. You must specify an absolute path.");
                }
                return System.Web.Hosting.HostingEnvironment.MapPath(path);
            }
            else
                return path;
        }
    }
}
