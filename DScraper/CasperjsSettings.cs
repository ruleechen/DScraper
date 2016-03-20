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
    }
}
