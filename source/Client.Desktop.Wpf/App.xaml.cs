using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Desktop.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public IHost AppHost { get; set; }
        
        public App()
        {
            _application = this;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine(args.LoadedAssembly);
        }
    }

    public partial class App
    {
        private static App _application;
        public static string Title => GetAppTitle();
        public static string VersionNumber { get; } = GetVersionNumber();

        private static string GetVersionNumber()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }

        private static string GetAppTitle()
        {
            
            return $"EVC Prover" +
                   $" - v{GetVersionNumber()}" +
                   $" - { _application.AppHost.Services.GetService<IHostEnvironment>().EnvironmentName }";
        }
    }
}