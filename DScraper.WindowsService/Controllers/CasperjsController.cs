using Newtonsoft.Json;
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
    public class CasperjsController : ApiController
    {
        private const string CasperScripts_FolderName = "CasperScripts";

        [HttpGet]
        public HttpResponseMessage Get(string outputEncoding = null, string timeout = null)
        {
            return Core(false, outputEncoding, timeout);
        }

        [HttpPost]
        public HttpResponseMessage Post(string outputEncoding = null, string timeout = null)
        {
            return Core(true, outputEncoding, timeout);
        }

        private HttpResponseMessage Core(bool isPost, string outputEncoding = null, string timeout = null)
        {
            if (string.IsNullOrWhiteSpace(outputEncoding))
            {
                outputEncoding = Encoding.UTF8.WebName;
            }

            if (string.IsNullOrWhiteSpace(timeout))
            {
                timeout = TimeSpan.Zero.ToString();
            }

            var result = new HttpResponseMessage();

            var root = AppDomain.CurrentDomain.BaseDirectory;
            var tempFolder = Path.Combine(root, "Temporary");
            if (!Directory.Exists(tempFolder)) { Directory.CreateDirectory(tempFolder); }

            var tempName = Guid.NewGuid().ToString("N");
            var scriptFile = Path.Combine(root, tempFolder, tempName + ".js");
            var resultFile = Path.Combine(root, tempFolder, tempName + ".txt");
            var outputEncodingObj = Encoding.GetEncoding(outputEncoding);
            var timeoutObj = TimeSpan.Parse(timeout);
            var proxyLog = true;

            try
            {
                var scriptContent = string.Empty;
                if (Request.GetRouteData().Values.ContainsKey("file"))
                {
                    var file = (Request.GetRouteData().Values["file"] ?? string.Empty).ToString();
                    scriptContent = File.ReadAllText(Path.Combine(root, CasperScripts_FolderName, file));
                }
                else if (isPost)
                {
                    scriptContent = Request.Content.ReadAsStringAsync().Result;
                }
                File.WriteAllText(scriptFile, scriptContent);

                var dict = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                var jsonArg = JsonConvert.SerializeObject(dict);

                var arguments = new List<string>();
                arguments.Add(scriptFile);
                arguments.Add(resultFile);
                arguments.Add(jsonArg);
                arguments.Add(outputEncodingObj.WebName);
                arguments.Add(timeoutObj.ToString());
                arguments.Add(proxyLog.ToString().ToLower());

                var executableName = typeof(Executable.Program).Assembly.ManifestModule.Name;
                var executableFile = Path.Combine(root, executableName);

                using (var p = new Process())
                {
                    p.StartInfo.FileName = executableFile;
                    p.StartInfo.Arguments = string.Join(" ", arguments.Select(x => "\"" + HttpUtility.UrlEncode(x) + "\""));
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

                result.StatusCode = HttpStatusCode.OK;
                result.Content = new StringContent(scrapeResult);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                var type = typeof(CasperjsController);
                var source = type.Namespace + "." + type.Name;
                EventLog.WriteEntry(source, ex.ToString());
                LogFactory.GetLogger().Error(source, ex);

                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Content = new StringContent(ex.ToString());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
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
