using System;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using DcrWebService;
using Devices.Core.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.Exporter;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Modules.UnionGas.Login;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas
{
    public static class MainMenuItems
    {
        public static IMainMenuItem ExporterMainMenu
            => new MainMenu("Export Test Run", PackIconKind.CloudUpload, screen => screen.ChangeView<ExporterViewModel>(), 4);
    }

    public class Startup : IConfigureModule
    {
        public void Configure(HostBuilderContext builder, IServiceCollection services)
        {
            services.AddSingleton<IMainMenuItem>(c => MainMenuItems.ExporterMainMenu);
            services.AddSingleton<IToolbarItem, LoginToolbarViewModel>();

            services.AddViewsAndViewModels();

            AddMasaWebService(services);

            services.AddSingleton<ILoginService<EmployeeDTO>, LoginService>();

            services.AddScoped<IExportVerificationTest, ExportToMasaManager>();

            services.AddSingleton<Func<DeviceInstance, EvcVerificationTest>>(c =>
                (device) => new UnionGasEvcVerification(device, "", c.GetService<ILoginService<EmployeeDTO>>().User?.Id));

            //        builder.RegisterType<CompanyNumberValidationManager>()
            //            .As<IEvcDeviceValidationAction>()
            //            .AsSelf();

            //        builder.RegisterType<UserLoggedInValidator>().As<IValidator>();
        }

        private void AddMasaWebService(IServiceCollection services)
        {
            services.AddSingleton<DCRWebServiceSoap>(c =>
                new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap));

            services.AddSingleton<DCRWebServiceCommunicator>();
        }
    }
}