using Client.Wpf.Extensions;
using Client.Wpf.Screens;
using Client.Wpf.ViewModels;
using Client.Wpf.Views;
using Client.Wpf.Views.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using ReactiveUI;
using Splat;

namespace Client.Wpf.Startup
{
    public class UserInterface
    {
        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            services.UseMicrosoftDependencyResolver();

            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            services.AddSingleton<IWindowFactory, WindowFactory>();

            services.AddSingleton(c => new MainViewModel(c, c.GetService<IDialogService>()));
            services.AddSingleton<IScreen, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<IScreenManager, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<HomeView>();

            services.AddViewsAndViewModels();
            services.AddMainMenuItems();

            AddDialogs(services);
        }

        private static void AddDialogs(IServiceCollection services)
        {
            services.AddSingleton<IDialogService>(c =>
                new DialogService(dialogTypeLocator: new DialogTypeLocator(c), dialogFactory: new DialogFactory(c))
            );


            services.AddScoped<BackgroundWorkDialog>();
        }
    }
}