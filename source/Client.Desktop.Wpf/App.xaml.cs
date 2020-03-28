using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace Client.Desktop.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public IHost AppHost { get; set; }

        public static string VersionNumber { get; } = GetVersionNumber();

        public App()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine(args.LoadedAssembly);
        }

        private static string GetVersionNumber()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }

    }
}