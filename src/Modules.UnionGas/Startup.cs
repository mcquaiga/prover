using System.Reactive;
using Client.Wpf.Extensions;
using Client.Wpf.Screens;
using Client.Wpf.Startup;
using Client.Wpf.ViewModels;
using Client.Wpf.ViewModels.Clients;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.UnionGas.Exporter.Views;
using ReactiveUI;

namespace Modules.UnionGas
{
    internal static class MainMenuItems
    {
        public static IMainMenuItem ExporterMainMenu(IScreenManager screenManager)
            => new MainMenu(screenManager, "Export Test Run", PackIconKind.CloudUpload, screen => screen.ChangeView<ExporterViewModel>(), 4);
    }


    public class Startup : IConfigureModule
    {
        public void Configure(HostBuilderContext builder, IServiceCollection services)
        {
            //services.
            services.AddViewsAndViewModels();

        }
    }
}