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
            var scraper = new CasperjsScraper();

            var script = root + @"\scripts\test.js";
            var result = scraper.Execute(script, new { test = 1 });
            Console.WriteLine(result);
        }
    }
}
