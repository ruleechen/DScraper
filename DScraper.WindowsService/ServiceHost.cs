using Microsoft.Owin.Hosting;
using System;
using System.ServiceProcess;

namespace DScraper.WindowsService
{
    public partial class ServiceHost : ServiceBase
    {
        public ServiceHost()
        {
            InitializeComponent();
            ServiceName = Constants.SERVICE_NAME;
        }

        IDisposable ApiHost;

        protected override void OnStart(string[] args)
        {
            var apiBaseAddress = Constants.ApiBaseAddress;

            var config = ScraperExtensions.Configuration.AppSettings.Settings["DScraper.ApiBaseAddress"];

            if (config != null)
            {
                apiBaseAddress = config.Value;
            }

            ApiHost = WebApp.Start<Startup>(url: apiBaseAddress);
        }

        protected override void OnStop()
        {
            if (ApiHost != null)
            {
                ApiHost.Dispose();
            }
        }
    }
}
