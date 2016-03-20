using System;
using System.IO;
using System.Configuration;
using System.Linq;

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
            var element = ScraperExtensions.Current.AppSettings.Settings["DScraper.CasperjsExePath"];

            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return ScraperExtensions.GetExecutableFullPath("casperjs.exe");
            }
        }

        public static void VerifyEnvironment()
        {
            var python = ScraperExtensions.GetExecutableFullPath("python.exe");

            if (string.IsNullOrEmpty(python))
            {
                throw new FileNotFoundException("Environment can not found python executable file");
            }

            var phantomjs = ScraperExtensions.GetExecutableFullPath("phantomjs.exe");

            if (string.IsNullOrEmpty(phantomjs))
            {
                throw new FileNotFoundException("Environment can not found phantomjs executable file");
            }

            var casperjs = ScraperExtensions.GetExecutableFullPath("casperjs.exe");

            if (string.IsNullOrEmpty(casperjs))
            {
                throw new FileNotFoundException("Environment can not found casperjs executable file");
            }
        }
    }
}
