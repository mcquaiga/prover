using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Client.Wpf.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.EventLog;
using ReactiveUI;
using Splat;

namespace Client.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private const string Environment = "Development";

        public IHost AppHost { get; set; }

        public App()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        public IServiceProvider Container => AppHost.Services;

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine(args.LoadedAssembly);
        }

    }
}