using Application.Services;
using Client.Wpf.Extensions;
using Client.Wpf.Startup;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Shared.Interfaces;
using Splat;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Client.Wpf
{
    public class AppBootstrapper 
    {
        public IConfiguration Config { get; set; }

        #region IHostedService Members

        #endregion

        public void AddServices(IServiceCollection services)
        {
            Settings.AddServices(services, Config);
            Storage.AddServices(services, Config);
            UserInterface.AddServices(services, Config);
        }

        public void ConfigureAppConfiguration(IConfigurationBuilder configHost)
        {
            var fp = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            configHost.SetBasePath(Directory.GetCurrentDirectory());
            
            // configHost.AddJsonFile("appsettings.json", false);
            //configHost.AddEnvironmentVariables(prefix: "PREFIX_");
            //configHost.AddCommandLine(args);
        }
    }

    
}