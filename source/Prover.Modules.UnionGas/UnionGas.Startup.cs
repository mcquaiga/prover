using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Infrastructure.KeyValueStore;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber;
using Prover.Modules.UnionGas.Login;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Modules.UnionGas.VerificationEvents;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Prover.Modules.UnionGas
{
    public class ExporterMainMenu : IMainMenuItem
    {
        public ExporterMainMenu(IScreenManager screenManager)
        {
            OpenCommand =
                ReactiveCommand.CreateFromTask(async () =>
                {
                    await screenManager.ChangeView<ExporterViewModel>();
                    return;
                });
        }

        public PackIconKind MenuIconKind { get; } = PackIconKind.CloudUpload;
        public string MenuTitle { get; } = "Export Test Run";
        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public int? Order { get; } = 2;
    }

    public class UnionGasModule : IConfigureModule
    {
        public void Configure(HostBuilderContext builder, IServiceCollection services)
        {
            services.AddSingleton<IMainMenuItem, ExporterMainMenu>();
            services.AddSingleton<IToolbarItem, LoginToolbarViewModel>();

            services.AddViewsAndViewModels();
         
            services.AddSingleton<Func<EvcVerificationTest, ExporterViewModel, VerificationGridViewModel>>(c =>
                (evcTest, exporter)
                    => new VerificationGridViewModel(
                        c.GetService<ILogger<VerificationGridViewModel>>(),
                        evcTest,
                        c.GetService<IVerificationTestService>(),
                        c.GetService<ILoginService<EmployeeDTO>>(),
                        c.GetService<IExportVerificationTest>(),
                        exporter
                    ));

           
            services.AddSingleton<MasaLoginService>();
            services.AddSingleton<ILoginService<EmployeeDTO>>(c => c.GetRequiredService<MasaLoginService>());
            services.AddSingleton<ILoginService>(c => c.GetRequiredService<MasaLoginService>());
            //services.AddHostedService<MasaLoginService>();

            services.AddSingleton<IExportVerificationTest, ExportToMasaManager>();
            services.AddTransient<TestsByJobNumberViewModel>();
            services.AddSingleton<IRepository<string, EmployeeDTO>, LiteDbRepository<string, EmployeeDTO>>();
            
            AddVerificationActions(services);

            if (builder.HostingEnvironment.IsDevelopment())
                DevelopmentServices(services);
            else
                AddMasaWebService(services);
        }

        private void AddVerificationActions(IServiceCollection services)
        {
            services.AddSingleton<MeterInventoryNumberValidator>();
            //services.AddSingleton<MasaVerificationActions>();

            services.AddAllTypes<IEventsSubscriber>(lifetime: ServiceLifetime.Singleton);
            //services.AddSingleton<IOnInitializeAction>(c => c.GetRequiredService<MasaVerificationActions>());
            //services.AddSingleton<IOnSubmitAction>(c => c.GetRequiredService<MasaVerificationActions>());
            //services.AddSingleton<IVerificationAction>(c => c.GetRequiredService<MasaVerificationActions>());
        }

        private void AddMasaWebService(IServiceCollection services, string remoteAddress = null)
        {
            services.AddSingleton<DCRWebServiceSoap>(c =>
                string.IsNullOrEmpty(remoteAddress)
                    ? new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap)
                    : new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap,
                        remoteAddress));

            services.AddSingleton<MasaService>();
            services.AddSingleton<IUserService<EmployeeDTO>>(c => c.GetRequiredService<MasaService>());
            services.AddSingleton<IMeterService<MeterDTO>>(c => c.GetRequiredService<MasaService>());
            services.AddSingleton<IExportService<QARunEvcTestResult>>(c => c.GetRequiredService<MasaService>());
        }

        private void DevelopmentServices(IServiceCollection services)
        {
            services.AddSingleton<DevelopmentWebService>();
            services.AddSingleton<IUserService<EmployeeDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
            services.AddSingleton<IMeterService<MeterDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
            services.AddSingleton<IExportService<QARunEvcTestResult>>(c => c.GetRequiredService<DevelopmentWebService>());
        }

    }
}