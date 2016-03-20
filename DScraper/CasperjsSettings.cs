using System;
using System.Collections.Generic;
using System.IO;

namespace DScraper
{
    public class CasperjsSettings
    {
        public string CasperjsExePath { get; set; }

        private static CasperjsSettings _fromDetection;
        public static CasperjsSettings FromDetection
        {
            get
            {
                if (_fromDetection == null)
                {
                    _fromDetection = new CasperjsSettings
                    {
                        CasperjsExePath = GetCasperjsExePath()
                    };
                }

                return _fromDetection;
            }
        }

        private static string GetCasperjsExePath()
        {
            var element = ScraperExtensions.Configuration.AppSettings.Settings["DScraper.CasperjsExePath"];

            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return ScraperExtensions.GetExecutableFullPath("casperjs.exe");
            }
        }

        private const string VerificationCacheKey = "DSCRAPER_ENVIRONMENT_VERIFICATION";

        public static void VerifyEnvironment()
        {
            if ((string)AppDomain.CurrentDomain.GetData(VerificationCacheKey) == "1")
            {
                return;
            }

            var executables = new List<string>
            {
                "python.exe",
                "phantomjs.exe",
                "casperjs.exe"
            };

            foreach (var file in executables)
            {
                var path = ScraperExtensions.GetExecutableFullPath("python.exe");

                if (string.IsNullOrEmpty(path))
                {
                    throw new FileNotFoundException("Environment can not found executable file: " + file);
                }
            }

            AppDomain.CurrentDomain.SetData(VerificationCacheKey, "1");
        }
    }
}
