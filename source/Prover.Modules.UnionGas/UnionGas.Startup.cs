using System;
using System.Collections.Generic;
using System.Linq;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Reports;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddSingleton<Func<EvcVerificationTest, ExporterViewModel, VerificationGridViewModel>>(c => (evcTest, vm) =>
            {
                return new VerificationGridViewModel(evcTest,
                    c.GetService<IVerificationTestService>(),
                    c.GetService<EvcVerificationTestService>(),
                    c.GetService<VerificationTestReportGenerator>(),
                    c.GetService<ILoginService<EmployeeDTO>>(),
                    c.GetService<IExportVerificationTest>(),
                    vm
                );
            });

            AddWebService(services);
            
            if (builder.HostingEnvironment.IsDevelopment()) 
                DevelopmentServices(services);
            else
            {
                ProductionServices(services);
            }
        }

        private void DevelopmentServices(IServiceCollection services)
        {
            AddLocalLoginService(services);
            services.AddSingleton<IExportVerificationTest, ExportManager>();
            services.AddSingleton<IVerificationCustomActions, DevVerificationInitializer>();
        }

        private void ProductionServices(IServiceCollection services)
        {
            services.AddSingleton<ILoginService<EmployeeDTO>, MasaLoginService>();
            services.AddSingleton<IVerificationCustomActions, MasaVerificationInitialization>();
            services.AddSingleton<IExportVerificationTest, ExportToMasaManager>();
        }

        private void AddWebService(IServiceCollection services, string remoteAddress = null)
        {
            services.AddSingleton<DCRWebServiceSoap>(c =>
                string.IsNullOrEmpty(remoteAddress) 
                    ? new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap) 
                    : new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap, remoteAddress));
        }

        private void AddLocalLoginService(IServiceCollection services)
        {
            var employeesLists = new List<EmployeeDTO>
            {
                new EmployeeDTO {EmployeeName = "Adam McQuaig", EmployeeNbr = "123", Id = "1"},
                new EmployeeDTO {EmployeeName = "Tony", EmployeeNbr = "1234", Id = "2"},
                new EmployeeDTO {EmployeeName = "Glen", EmployeeNbr = "12345", Id = "3"},
                new EmployeeDTO {EmployeeName = "Kyle", EmployeeNbr = "123456", Id = "4"}
            };

            services.AddSingleton<ILoginService<EmployeeDTO>, LocalLoginService<EmployeeDTO>>(c =>
            {
                return new LocalLoginService<EmployeeDTO>(employeesLists,
                    (employees, id) => employees.FirstOrDefault(e => e.EmployeeNbr == id));
            });
        }
    }
}
