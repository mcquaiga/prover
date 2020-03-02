using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Modules.UnionGas.Exporter.Views;

namespace Prover.Modules.UnionGas
{
    internal static class MainMenuItems
    {
        public static IMainMenuItem ExporterMainMenu(IScreenManager screenManager)
            => new MainMenu("Export Test Run", PackIconKind.CloudUpload, screen => screen.ChangeView<ExporterViewModel>(), 4);
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