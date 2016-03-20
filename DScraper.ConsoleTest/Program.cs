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
            var scraper = new CasperjsScraper();

            var script = AppDomain.CurrentDomain.BaseDirectory + @"\scripts\test.js";
            var result = scraper.Execute(script, new { test = "rulee" });
            Console.WriteLine(result);
        }
    }
}
