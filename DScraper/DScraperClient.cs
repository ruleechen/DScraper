using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DScraper
{
    public class DScraperClient
    {
        public static void Execute()
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var csp = root + @"\tools\casperjs\";
            var pht = root + @"\tools\phantomjs\";
            var env = string.Format(";{0};{1}", pht, csp);

            var csp1 = new FileInfo(root + @"\tools\casperjs\bin\casperjs.exe");
            var exe = new FileInfo(@"D:\Program Files\Python\Python35-32\python.exe");

            var script = root + @"\scripts\test.js";
            var command = "casperjs " + script;
            var result = ExecutePythonScript(exe, csp1.Directory, command, env);

            Console.WriteLine(result);
        }

        public static Encoding DetectEncoding(String fileName, out String contents)
        {
            // open the file with the stream-reader:
            using (StreamReader reader = new StreamReader(fileName, true))
            {
                // read the contents of the file into a string
                contents = reader.ReadToEnd();

                // return the encoding.
                return reader.CurrentEncoding;
            }
        }

        private static string ExecutePythonScript(FileInfo pythonPath, DirectoryInfo workingDir, string casperArguments, string EnvPath)
        {
            var p = new Process();
            p.StartInfo.EnvironmentVariables["PATH"] = EnvPath;
            p.StartInfo.WorkingDirectory = workingDir.FullName;
            p.StartInfo.FileName = pythonPath.FullName;
            p.StartInfo.Arguments = casperArguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            var result = string.Empty;

            p.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    result += e.Data;
                }
            };

            p.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    result += e.Data;
                }
            };

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.Close();

            return result;
        }
    }
}
