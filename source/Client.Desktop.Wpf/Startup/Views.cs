using System;
using Client.Desktop.Wpf.Controls;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Reports;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interfaces;
using ReactiveUI;
using Splat;


namespace Client.Desktop.Wpf.Startup
{
    public class UserInterface
    {
        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.UseMicrosoftDependencyResolver();
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            services.AddSingleton<ICreatesObservableForProperty, CustomPropertyResolver>();
            services.AddSingleton<IWindowFactory, WindowFactory>();
            services.AddSingleton(c => new MainWindow());

            //var homeViewModelFactory = (s) => new HomeViewModel(s, )
            //services.AddSingleton<IViewFor<HomeViewModel>>(c => new HomeView());

            services.AddSingleton<Func<IScreenManager, HomeViewModel>>(c =>
                (screen) => new HomeViewModel(screen, c.GetServices<IMainMenuItem>()));

            services.AddSingleton<HomeViewModel>(c =>
                c.GetService<Func<IScreenManager, HomeViewModel>>().Invoke(c.GetService<IScreenManager>()));

            //services.AddSingleton(c => new HomeViewModel(c.GetService<IScreenManager>(), c.GetServices<IMainMenuItem>()));

            services.AddSingleton(c => new MainViewModel(c, c.GetService<IDialogServiceManager>(), c.GetService<Func<IScreenManager, HomeViewModel>>()));
            //services.AddSingleton<MainViewModel>();
            services.AddSingleton<IScreen, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<IScreenManager, MainViewModel>(c => c.GetService<MainViewModel>());

            services.AddMainMenuItems();
            services.AddViewsAndViewModels();

            AddDialogs(services);
            AddReporting(services);
        }

        private static void AddReporting(IServiceCollection services)
        {
            services.AddSingleton<VerificationTestReportGenerator>();
        }

        private static void AddDialogs(IServiceCollection services)
        {
            services.AddSingleton<IDialogServiceManager, DialogServiceManager>();
            //services.AddDialogViews();
        }
    }

}