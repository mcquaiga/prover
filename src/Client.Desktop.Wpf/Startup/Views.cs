using Client.Desktop.Wpf.Controls;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Reports;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.ViewModels.Verifications;
using Client.Desktop.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            //services.AddSingleton(c => new MainViewModel(c, c.GetRequiredService<IDialogService>()));
            //services.AddSingleton<ReactiveDialog<DialogManager>, DialogUserControl>(c => new DialogUserControl(){ ViewModel = (DialogManager)c.GetService<IDialogService>()});
            
            services.AddSingleton(c => new MainWindow());
            services.AddSingleton(c => new MainViewModel(c, c.GetService<DialogServiceManager>()));
            services.AddSingleton<IScreen, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<IScreenManager, MainViewModel>(c => c.GetService<MainViewModel>());

            //services.AddSingleton<ITestManagerViewModelFactory, TestManagerViewModel>();

            services.AddMainMenuItems();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton(c => new HomeView());

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