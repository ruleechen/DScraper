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
            ApiHost = WebApp.Start<Startup>(url: Constants.ApiBaseAddress);
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
