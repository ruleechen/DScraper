using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DScraper.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var scraper = new CasperjsScraper(new CasperjsSettings
            {
                PythonExePath = (@"D:\Program Files\Python\Python35-32\python.exe"),
                PhantomjsExePath = (root + @"\tools\phantomjs\phantomjs.exe"),
                CasperjsExePath = (root + @"\tools\casperjs\bin\casperjs.exe")
            });

            var script = root + @"\scripts\test.js";
            var result = scraper.Execute(script, new { test = 1 });
            Console.WriteLine(result);
        }
    }
}
