using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Client.Wpf.Screens;
using Client.Wpf.ViewModels;
using Client.Wpf.ViewModels.Dialogs;
using Client.Wpf.Views.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using ReactiveUI;

namespace Client.Wpf
{
    public class DialogFactory : IDialogFactory
    {
        private readonly IServiceProvider _services;

        public DialogFactory(IServiceProvider services)
        {
            _services = services;
        }
        
        public IWindow Create(Type dialogType)
        {
            return (IWindow)_services.GetService(dialogType);
        }
    }

    public class AppBootstrapper
    {
        public static async Task ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWindowFactory, WindowFactory>();

            services.AddSingleton<IDialogService>(c => 
                new DialogService(dialogTypeLocator: new DialogTypeLocator(c), dialogFactory: new DialogFactory(c))
            );

            services.AddSingleton(c => new MainViewModel(c, c.GetService<IDialogService>()));
            services.AddSingleton<IScreen, MainViewModel>(c => c.GetService<MainViewModel>());
            services.AddSingleton<IScreenManager, MainViewModel>(c => c.GetService<MainViewModel>());

            services.AddScoped<BackgroundWorkDialog>();
            //services.AddScoped<BackgroundWorkDialogViewModel>();
            //services.AddSingleton<IViewFor<MainViewModel>, MainWindow>();

            //c.GetRequiredService<IEnumerable<IMainMenuItem>>()

            //services.AddSingleton(c => new HomeViewModel(c.GetService<IScreenManager>(), c.GetServices<IMainMenuItem>()));
            //services.AddScoped<IViewFor<HomeViewModel>, HomeView>();

            //services.AddScoped(c => new VerificationTestViewModel(c.GetService<IScreenManager>()));
            //services.AddScoped<IMainMenuItem, VerificationTestViewModel>();
            //services.AddScoped<IViewFor<VerificationTestViewModel>, VerificationTestView>();

            services.AddViewsAndViewModels();

            //await Devices.Repository.GetAsync();
        }

        public static void ConfigureAppConfiguration(IConfigurationBuilder config)
        {
        }
    }

    public static class ServiceExtensions
    {
        public static void AddViewsAndViewModels(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            // for each type that implements IViewFor
            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
            {
                // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
                var ivf = ti.ImplementedInterfaces.FirstOrDefault(t =>
                    t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));


                // need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
                if (ivf != null && !ivf.ContainsGenericParameters) 
                    services.TryAddScoped(ivf, ti);
            }


            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IRoutableViewModel)) && !ti.IsAbstract))
                if (!ti.ImplementedInterfaces.Contains(typeof(IScreen)))
                {
                    services.AddScoped(ti, ti);
                    services.AddScoped(typeof(IRoutableViewModel), ti);
                }
            // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>

            assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IMainMenuItem)) && !ti.IsAbstract).ToList()
                .ForEach(ti => services.AddScoped(typeof(IMainMenuItem), ti));
        }
    }
}