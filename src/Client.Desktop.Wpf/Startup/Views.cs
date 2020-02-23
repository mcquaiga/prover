using Client.Desktop.Wpf.ViewModels;
using Client.Wpf.Controls;
using Client.Wpf.Extensions;
using Client.Wpf.Screens.Dialogs;
using Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;

namespace Client.Wpf.Startup
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

            //services.AddSingleton(c => new MainViewModel(c, c.GetRequiredService<IDialogService>()));
            //services.AddSingleton<ReactiveDialog<DialogManager>, DialogUserControl>(c => new DialogUserControl(){ ViewModel = (DialogManager)c.GetService<IDialogService>()});

            

            services.AddSingleton(c => new MainWindow());
            services.AddSingleton(c => new MainViewModel(c, c.GetService<DialogServiceManager>()));
            services.AddSingleton<IScreen, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<IScreenManager, MainViewModel>(c => c.GetService<MainViewModel>());

            services.AddMainMenuItems();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton(c => new HomeView());

            services.AddViewsAndViewModels();

            AddDialogs(services);
        }

        private static void AddDialogs(IServiceCollection services)
        {
            //services.AddSingleton<IDialogService>(c =>
            //    new DialogService(dialogTypeLocator: new DialogTypeLocator(c), dialogFactory: new DialogFactory(c)));

            //services.AddSingleton<IDialogService>(c =>
            //    new DialogManager(c, dialogTypeLocator: new DialogTypeLocator(c), dialogFactory: new DialogFactory(c)));
            services.AddSingleton(c =>
                new DialogServiceManager(c));

            services.AddDialogViews();

            //services.AddScoped<BackgroundWorkDialog>();
        }
    }
}