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
        static DScraperClient()
        {

        }

        public static void Execute(string script)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;

            var csp = root + @"\tools\casperjs\bin";
            var pht = root + @"\tools\phantomjs\";
            var env = string.Format(";{0};{1}", pht, csp);

            var csp1 = new FileInfo(root + @"\tools\casperjs\bin\casperjs.exe");
            var exe = new FileInfo(@"D:\Program Files\Python\Python35-32\python.exe");

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

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        private static string ExecutePythonScript(FileInfo pythonPath, DirectoryInfo workingDir, string casperArguments, string EnvPath)
        {
            var result = string.Empty;

            using (var p = new Process())
            {
                p.StartInfo.EnvironmentVariables["PATH"] = EnvPath;
                p.StartInfo.WorkingDirectory = workingDir.FullName;
                p.StartInfo.FileName = pythonPath.FullName;
                p.StartInfo.Arguments = casperArguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;

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
            }

            return result;
        }
    }
}
