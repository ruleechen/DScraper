using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DScraper
{
    public class CasperjsScraper
    {
        private CasperjsSettings _settings;

        public CasperjsScraper()
            : this(CasperjsSettings.FromConfig)
        {
        }

        public CasperjsScraper(CasperjsSettings settings)
        {
            _settings = settings;

            if (_settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(_settings.PythonExePath))
            {
                throw new ArgumentNullException(nameof(_settings.PythonExePath));
            }

            if (string.IsNullOrWhiteSpace(_settings.PhantomjsExePath))
            {
                throw new ArgumentNullException(nameof(_settings.PhantomjsExePath));
            }

            if (string.IsNullOrWhiteSpace(_settings.CasperjsExePath))
            {
                throw new ArgumentNullException(nameof(_settings.CasperjsExePath));
            }

            Debug = false;
            DebugPort = 9001;
            DebugRemote = "http://localhost";
            OutputEncoding = Encoding.GetEncoding("GB2312");
        }

        public bool Debug { get; set; }

        public int DebugPort { get; set; }

        public string DebugRemote { get; set; }

        public Encoding OutputEncoding { get; set; }

        public string Execute(string scriptPath, object arg = null)
        {
            var flags = new List<string>();
            var arguments = new List<string>();
            var arg0 = JsonConvert.SerializeObject(arg ?? new { });

            if (!string.IsNullOrEmpty(arg0))
            {
                arguments.Add(HttpUtility.UrlEncode(arg0));
            }

            if (Debug)
            {
                flags.Add("--remote-debugger-port=" + DebugPort);

                if (!HttpContext.Current.IsAvailable())
                {
                    StartWebkitDebug();
                }
            }

            if (OutputEncoding != null)
            {
                flags.Add("--output-encoding=" + OutputEncoding.HeaderName);
            }

            var command = string.Format("casperjs {0} {1} {2}",
                string.Join(" ", flags), scriptPath, string.Join(" ", arguments));

            var result = ExecutePythonScript(command);

            return result;
        }

        public T Get<T>(string scriptPath, object args = null)
        {
            var result = Execute(scriptPath, args);

            return JsonConvert.DeserializeObject<T>(result);
        }

        private string ExecutePythonScript(string casperjsCommand)
        {
            var casperjsDirectory = Path.GetDirectoryName(_settings.CasperjsExePath);
            var phantomjsDirectory = Path.GetDirectoryName(_settings.PhantomjsExePath);
            var PATH = string.Format(";{0};{1}", casperjsDirectory, phantomjsDirectory);

            var result = string.Empty;
            using (var p = new Process())
            {
                p.StartInfo.EnvironmentVariables["PATH"] = PATH;
                p.StartInfo.WorkingDirectory = casperjsDirectory;
                p.StartInfo.FileName = _settings.PythonExePath;
                p.StartInfo.Arguments = casperjsCommand;
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

        private void StartWebkitDebug()
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                try
                {
                    Process.Start("chrome.exe", DebugRemote + ":" + DebugPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can not open Google Chrome: " + ex.Message);
                }
            });
        }
    }
}
