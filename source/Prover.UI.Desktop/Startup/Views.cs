﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interfaces;
using Prover.UI.Desktop.Controls;
using Prover.UI.Desktop.Dialogs;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Reports;
using Prover.UI.Desktop.ViewModels;
using Prover.UI.Desktop.ViewModels.Verifications;
using Prover.UI.Desktop.Views;
using ReactiveUI;
using Splat;

namespace Prover.UI.Desktop.Startup
{
    public class ProverViewLocator : IViewLocator
    {
        /// <inheritdoc />
        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class => throw new System.NotImplementedException();
    }

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

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IScreenManager, ScreenManager>();
            services.AddSingleton<IScreen>(c => c.GetRequiredService<IScreenManager>());


            services.AddMainMenuItems();
            services.AddViewsAndViewModels();

            services.AddSingleton<NewTestRunViewModel>();

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