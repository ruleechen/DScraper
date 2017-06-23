using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
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

            try
            {
                var content = Request.Content.ReadAsStringAsync().Result;
                File.WriteAllText(scriptFile, content);

                var scraper = new CasperScraper();
                var scrapeResult = scraper.Execute(scriptFile);

                result.Content = new StringContent(scrapeResult, scraper.OutputEncoding);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("DScraper.WindowsService.CasperController", ex.ToString());
            }
            finally
            {
                if (File.Exists(scriptFile))
                {
                    File.Delete(scriptFile);
                }
            }

            return result;
        }
    }
}
