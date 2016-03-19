using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DScraper
{
    public class CasperjsSettings
    {
        public string PythonExePath { get; set; }
        public string PhantomjsExePath { get; set; }
        public string CasperjsExePath { get; set; }

        private static CasperjsSettings _default;
        public static CasperjsSettings Default
        {
            get
            {
                if (_default == null)
                {
                    var rootPath = AppDomain.CurrentDomain.BaseDirectory;

                    Initialize(rootPath);

                    _default = new CasperjsSettings
                    {
                        PythonExePath = Path.Combine(rootPath, @"\tools\python\python.exe"),
                        PhantomjsExePath = Path.Combine(rootPath, @"\tools\phantomjs\phantomjs.exe"),
                        CasperjsExePath = Path.Combine(rootPath, @"\tools\casperjs\bin\casperjs.exe")
                    };
                }

                return _default;
            }
        }

        private static void Initialize(string rootPath)
        {
            //TODO:
        }
    }
}
