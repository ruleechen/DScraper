using Newtonsoft.Json;
using System;
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
        public HttpResponseMessage Post()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var root = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var fileName = Guid.NewGuid().ToString("N");
            var scriptFile = Path.Combine(root, fileName + ".js");
            var resultFile = Path.Combine(root, fileName + ".txt");
            var encoding = Encoding.GetEncoding("GB2312");

            try
            {
                var content = Request.Content.ReadAsStringAsync().Result;
                File.WriteAllText(scriptFile, content);

                var dict = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                var jsonArg = HttpUtility.UrlEncode(JsonConvert.SerializeObject(dict));

                using (var p = new Process())
                {
                    p.StartInfo.FileName = Path.Combine(root, "DScraper.Executable.exe");
                    p.StartInfo.Arguments = "\"" + scriptFile + "\" \"" + resultFile + "\" \"" + jsonArg + "\" \"false\"";
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

                var scrapeResult = File.ReadAllText(resultFile, encoding);
                result.Content = new StringContent(scrapeResult, encoding);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                var type = typeof(CasperController);
                var source = type.Namespace + "." + type.Name;
                EventLog.WriteEntry(source, ex.ToString());
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
