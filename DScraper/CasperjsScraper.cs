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

            if (string.IsNullOrWhiteSpace(_settings.CasperjsExePath))
            {
                throw new ArgumentNullException(nameof(_settings.CasperjsExePath));
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

        public string Execute(string scriptPath, object arg = null)
        {
            var flags = new List<string>();
            var arguments = new List<string>();
            var arg0 = JsonConvert.SerializeObject(arg ?? new { });

            if (!string.IsNullOrEmpty(arg0))
            {
                arguments.Add(HttpUtility.UrlEncode(arg0));
            }

            if (Debugger && !HttpContext.Current.IsAvailable())
            {
                flags.Add("--remote-debugger-port=" + DebuggerPort);

                StartWebkitDebugger(delay: 1000);
            }

            if (OutputEncoding != null)
            {
                flags.Add("--output-encoding=" + OutputEncoding.HeaderName);
            }

            var command = string.Format("casperjs {0} {1} {2}",
                string.Join(" ", flags), scriptPath, string.Join(" ", arguments));

            var timeout = ExecuteTimeout > TimeSpan.MinValue ? ExecuteTimeout : TimeSpan.FromMinutes(1);

            var result = ExecutePythonScript(command, timeout);

            return result;
        }

        public T Get<T>(string scriptPath, object args = null)
        {
            var result = Execute(scriptPath, args);

            return JsonConvert.DeserializeObject<T>(result);
        }

        private class WatchTaskModel
        {
            public Process Process { get; set; }
            public TimeSpan Timeout { get; set; }
            public CancellationToken Token { get; set; }
        }

        private string ExecutePythonScript(string casperjsCommand, TimeSpan timeout)
        {
            var result = string.Empty;
            using (var p = new Process())
            {
                CancellationTokenSource source = null;
                if (timeout > TimeSpan.MinValue)
                {
                    source = new CancellationTokenSource();
                    var task = Task.Factory.StartNew((state) =>
                    {
                        var param = (WatchTaskModel)state;
                        Thread.Sleep(param.Timeout);
                        if (!param.Token.IsCancellationRequested)
                        {
                            param.Process.Close();
                            param.Process.Dispose();
                        }
                    },
                    new WatchTaskModel
                    {
                        Process = p,
                        Timeout = timeout,
                        Token = source.Token
                    },
                    source.Token,
                    TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default);
                }

                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(_settings.CasperjsExePath);
                p.StartInfo.FileName = "python.exe";
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

                if (source != null)
                {
                    source.Cancel();
                    source.Dispose();
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
