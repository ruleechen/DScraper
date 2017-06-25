using Owin;
using System.Web.Http;

namespace DScraper.WindowsService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{file}",
                defaults: new { file = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
