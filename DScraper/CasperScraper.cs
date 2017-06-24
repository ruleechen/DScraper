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
    public class CasperScraper
    {
        static CasperScraper()
        {
            CasperSettings.VerifyEnvironment();
        }

        private CasperSettings _settings;

        public CasperScraper()
            : this(CasperSettings.FromDetection)
        {
        }

        public CasperScraper(CasperSettings settings)
        {
            _settings = settings;

            if (_settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(_settings.CasperjsExePath))
            {
                throw new ArgumentNullException("settings.CasperjsExePath");
            }

            Debugger = false;
            DebuggerPort = 9001;
            DebuggerRemote = "http://localhost";
            OutputEncoding = Encoding.GetEncoding("GB2312");
            ExecuteTimeout = TimeSpan.MinValue;
        }

        public bool Debugger { get; set; }

        public int DebuggerPort { get; set; }

        public string DebuggerRemote { get; set; }

        public Encoding OutputEncoding { get; set; }

        public TimeSpan ExecuteTimeout { get; set; }

        public string Execute(string scriptFile, object arg = null)
        {
            var jsonArg = JsonConvert.SerializeObject(arg ?? new { });
            return Execute(scriptFile, jsonArg);
        }

        public string Execute(string scriptFile, string jsonArg = null)
        {
            var flags = new List<string>();

            if (string.IsNullOrWhiteSpace(jsonArg))
            {
                jsonArg = "{}";
            }

            if (Debugger && !HttpContext.Current.IsAvailable())
            {
                flags.Add("--remote-debugger-port=" + DebuggerPort);

                StartWebkitDebugger(delay: 1000);
            }

            if (OutputEncoding != null)
            {
                flags.Add("--output-encoding=" + OutputEncoding.WebName);
            }

            var command = string.Format("casperjs {0} {1} {2}",
                string.Join(" ", flags), scriptFile, HttpUtility.UrlEncode(jsonArg));

            var result = ExecuteCasperScript(
                command: command,
                workingAt: Path.GetDirectoryName(_settings.CasperjsExePath),
                timeout: Debugger ? TimeSpan.MinValue : ExecuteTimeout);

            return result;
        }

        private static string ExecuteCasperScript(string command, string workingAt, TimeSpan timeout)
        {
            var result = string.Empty;

            using (var p = new Process())
            {
                ProcessWatcher watcher = null;

                if (timeout > TimeSpan.MinValue)
                {
                    watcher = p.Watch(timeout);

                    watcher.OnTimeout += (s, e) =>
                    {
                        result = "Process timeout";
                    };

                    watcher.StartWatch();
                }

                p.StartInfo.FileName = "python.exe";
                p.StartInfo.WorkingDirectory = workingAt;
                p.StartInfo.Arguments = command;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.UseShellExecute = false;
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

                if (watcher != null)
                {
                    watcher.Dispose();
                }
            }

            return result;
        }

        private void StartWebkitDebugger(int delay)
        {
            Task.Run(() =>
            {
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }

                try
                {
                    Process.Start("chrome.exe", DebuggerRemote + ":" + DebuggerPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can not open Google Chrome: " + ex.Message);
                }
            });
        }
    }
}
