﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DScraper
{
    public class CasperjsScraper
    {
        private CasperjsSettings _settings;

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

            if (string.IsNullOrWhiteSpace(_settings.CasperjsExePath))
            {
                throw new ArgumentNullException(nameof(_settings.CasperjsExePath));
            }

            if (string.IsNullOrWhiteSpace(_settings.PhantomjsExePath))
            {
                throw new ArgumentNullException(nameof(_settings.PhantomjsExePath));
            }
        }

        public string Execute(string scriptPath)
        {
            var command = "casperjs " + scriptPath;

            var result = ExecutePythonScript(command);

            return result;
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
