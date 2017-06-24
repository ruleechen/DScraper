﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Http;

namespace DScraper.WindowsService.Controllers
{
    public class CasperController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string message)
        {
            var res = new HttpResponseMessage(HttpStatusCode.OK);
            res.Content = new StringContent(message, Encoding.UTF8);
            return res;
        }

        [HttpPost]
        public HttpResponseMessage Post(string outputEncoding = null, string timeout = null)
        {
            if (string.IsNullOrWhiteSpace(outputEncoding))
            {
                outputEncoding = Encoding.UTF8.WebName;
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var root = AppDomain.CurrentDomain.BaseDirectory;

            var tempFolder = Path.Combine(root, "Temporary");
            if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }

            var tempName = Guid.NewGuid().ToString("N");
            var scriptFile = Path.Combine(root, tempFolder, tempName + ".js");
            var resultFile = Path.Combine(root, tempFolder, tempName + ".txt");
            var outputEncodingObj = Encoding.GetEncoding(outputEncoding);
            var proxyLog = false;

            try
            {
                var content = Request.Content.ReadAsStringAsync().Result;
                File.WriteAllText(scriptFile, content);

                var dict = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                var jsonArg = JsonConvert.SerializeObject(dict);

                var arguments = new List<string>();
                arguments.Add(scriptFile);
                arguments.Add(resultFile);
                arguments.Add(jsonArg);
                arguments.Add(outputEncodingObj.WebName);
                arguments.Add(timeout);
                arguments.Add(proxyLog.ToString().ToLower());

                var executableName = typeof(Executable.Program).Assembly.ManifestModule.Name;
                var executableFile = Path.Combine(root, executableName);

                using (var p = new Process())
                {
                    p.StartInfo.FileName = executableFile;
                    p.StartInfo.Arguments = string.Join(" ", arguments.Select(x => $"\"{HttpUtility.UrlEncode(x)}\""));
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    p.Close();
                }

                var scrapeResult = File.ReadAllText(resultFile);
                result.Content = new StringContent(scrapeResult);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                var type = typeof(CasperController);
                var source = type.Namespace + "." + type.Name;
                EventLog.WriteEntry(source, ex.ToString());
                LogFactory.GetLogger().Error(source, ex);
            }
            finally
            {
                if (File.Exists(scriptFile))
                {
                    File.Delete(scriptFile);
                }

                if (File.Exists(resultFile))
                {
                    File.Delete(resultFile);
                }
            }

            return result;
        }
    }
}
