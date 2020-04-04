using System;
using System.Collections.Generic;
using System.Linq;
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
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Modules.UnionGas.Login;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas
{
    public static class MainMenuItems
    {
        public static IMainMenuItem ExporterMainMenu
            => new MainMenu("Export Test Run", PackIconKind.CloudUpload,
                async screen => await screen.ChangeView<ExporterViewModel>(), 4);
    }

    public class Startup : IConfigureModule
    {
        public void Configure(HostBuilderContext builder, IServiceCollection services)
        {
            services.AddSingleton(c => MainMenuItems.ExporterMainMenu);
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

            services.AddSingleton<ILoginService<EmployeeDTO>, MasaLoginService>();
            services.AddSingleton<IExportVerificationTest, ExportToMasaManager>();
            
            AddVerificationActions(services);

            if (builder.HostingEnvironment.IsDevelopment())
                DevelopmentServices(services);
            else
                AddMasaWebService(services);
        }

        private void AddVerificationActions(IServiceCollection services)
        {
            services.AddSingleton<MeterInventoryNumberValidator>();
            services.AddSingleton<MasaVerificationActions>();
            services.AddSingleton<IInitializeAction>(c => c.GetRequiredService<MasaVerificationActions>());
            services.AddSingleton<ISubmitAction>(c => c.GetRequiredService<MasaVerificationActions>());
            services.AddSingleton<IVerificationAction>(c => c.GetRequiredService<MasaVerificationActions>());
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