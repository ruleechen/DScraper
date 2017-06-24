using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DScraper.Executable
{
    public class Program
    {
        static void Main(string[] args)
        {
            var decodedArgs = args.Select(x => HttpUtility.UrlDecode(x)).ToList();

            var scriptFile = decodedArgs[0];
            var resultFile = decodedArgs[1];
            var jsonArgument = decodedArgs[2];
            var outputEncoding = decodedArgs[3];
            var timeout = decodedArgs[4];
            var proxyLog = (decodedArgs[5] == "true");

            if (proxyLog)
            {
                var logPath = Path.GetDirectoryName(scriptFile);
                var logName = Path.GetFileNameWithoutExtension(typeof(Program).Assembly.ManifestModule.Name);
                var logFile = Path.Combine(logPath, logName + ".log");
                var logText = new List<string>(decodedArgs);
                logText.Insert(0, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                logText.Add(string.Empty);
                File.AppendAllLines(logFile, logText);
            }

            try
            {
                var scraper = new CasperScraper();

                if (!string.IsNullOrWhiteSpace(outputEncoding))
                {
                    scraper.OutputEncoding = Encoding.GetEncoding(outputEncoding);
                }

                if (!string.IsNullOrWhiteSpace(timeout))
                {
                    scraper.ExecuteTimeout = TimeSpan.Parse(timeout);
                }

                var scrapeResult = scraper.Execute(scriptFile, jsonArgument);

                File.WriteAllText(resultFile, scrapeResult);
            }
            catch (Exception ex)
            {
                File.WriteAllText(resultFile, ex.ToString());
            }
        }
    }
}
