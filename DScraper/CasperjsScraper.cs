using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
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
        }

        public string Execute(string scriptPath, object args = null)
        {
            args = args ?? new { };

            var arguments = JsonConvert.SerializeObject(args);

            arguments = HttpUtility.UrlEncode(arguments);

            var command = string.Format("casperjs --output-encoding=GB2312 {0} {1}", scriptPath, arguments);

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
    }
}
