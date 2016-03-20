using System;
using System.IO;
using System.Configuration;

namespace DScraper
{
    public class CasperjsSettings
    {
        public string PythonExePath { get; set; }
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
                        PythonExePath = DScraperExtensions.Current.AppSettings.Settings["DScraper.PythonExePath"].Value,
                        CasperjsExePath = DScraperExtensions.Current.AppSettings.Settings["DScraper.CasperjsExePath"].Value
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
