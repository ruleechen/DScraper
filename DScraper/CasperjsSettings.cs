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
                        PythonExePath = ConfigSource.Current.AppSettings.Settings["CasperjsScraper.PythonExePath"].Value,
                        PhantomjsExePath = ConfigSource.Current.AppSettings.Settings["CasperjsScraper.PhantomjsExePath"].Value,
                        CasperjsExePath = ConfigSource.Current.AppSettings.Settings["CasperjsScraper.CasperjsExePath"].Value
                    };
                }

                return _fromConfig;
            }
        }
    }
}
