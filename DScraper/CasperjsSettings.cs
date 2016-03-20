using System;
using System.IO;
using System.Configuration;

namespace DScraper
{
    public class CasperjsSettings
    {
        public string CasperjsExePath { get; set; }

        private static CasperjsSettings _fromConfig;
        public static CasperjsSettings FromConfig
        {
            get
            {
                if (_fromConfig == null)
                {
                    _fromConfig = new CasperjsSettings
                    {
                        CasperjsExePath = ScraperExtensions.Current.AppSettings.Settings["DScraper.CasperjsExePath"].Value
                    };
                }

                return _fromConfig;
            }
        }

        private static CasperjsSettings _fromProject;
        public static CasperjsSettings FromProject
        {
            get
            {
                if (_fromProject == null)
                {
                    _fromProject = new CasperjsSettings
                    {
                        //TODO:
                    };
                }

                return _fromProject;
            }
        }
    }
}
