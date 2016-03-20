using System;
using System.IO;
using System.Configuration;

namespace DScraper
{
    public class CasperjsSettings
    {
        public string PythonExePath { get; set; }
        public string PhantomjsExePath { get; set; }
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
                        PythonExePath = ConfigSource.Current.AppSettings.Settings["DScraper.PythonExePath"].Value,
                        PhantomjsExePath = ConfigSource.Current.AppSettings.Settings["DScraper.PhantomjsExePath"].Value,
                        CasperjsExePath = ConfigSource.Current.AppSettings.Settings["DScraper.CasperjsExePath"].Value
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
