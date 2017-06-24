using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace DScraper.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            var scriptFile = args[0];
            var resultFile = args[1];
            var jsonArgument = HttpUtility.UrlDecode(args[2]);
            var writeLog = (args[3] == "true");

            if (writeLog)
            {
                var logPath = Path.GetDirectoryName(scriptFile);
                var logName = Path.GetFileNameWithoutExtension(typeof(Program).Assembly.ManifestModule.Name);
                var logFile = Path.Combine(logPath, logName + ".log");
                var logText = new List<string>(args);
                logText.Insert(0, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                logText.Add(string.Empty);
                File.AppendAllLines(logFile, logText);
            }

            var scraper = new CasperScraper();
            var scrapeResult = scraper.Execute(scriptFile, jsonArgument);
            File.WriteAllText(resultFile, scrapeResult);
        }
    }
}
