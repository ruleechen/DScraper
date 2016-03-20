using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace DScraper
{
    public static class ScraperExtensions
    {
        private static Configuration _configuration;
        public static Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    if (HttpContext.Current.IsAvailable())
                    {
                        _configuration = WebConfigurationManager.OpenWebConfiguration("~");
                    }
                    else
                    {
                        _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    }
                }

                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        public static bool IsAvailable(this HttpContext context)
        {
            return context != null && context.Handler != null;
        }

        public static ProcessWatcher Watch(this Process process, TimeSpan timespan)
        {
            return new ProcessWatcher(process, timespan);
        }

        public static string GetExecutableFullPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            var PATH = Environment.GetEnvironmentVariable("PATH");

            foreach (var path in PATH.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
    }
}
