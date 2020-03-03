using System.ServiceModel;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using DcrWebService;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.UnionGas;
using Prover.Application.Interfaces;
using Prover.Modules.UnionGas.Exporter;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Shared.Interfaces;
using ReactiveUI;

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
            
            //services.AddViewsAndViewModels();
            services.AddScoped<IViewFor<ExporterViewModel>, ExporterView>();
            services.AddSingleton<ExporterViewModel>();

            services.AddSingleton<DCRWebServiceSoap>(c =>
                    new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap));

            services.AddSingleton<DCRWebServiceCommunicator>();

            services.AddSingleton<ILoginService<EmployeeDTO>, LoginService>();

            services.AddScoped<IExportVerificationTest, ExportToMasaManager>();
            

            //        builder.RegisterType<CompanyNumberValidationManager>()
            //            .As<IEvcDeviceValidationAction>()
            //            .AsSelf();

            //        builder.RegisterType<UserLoggedInValidator>().As<IValidator>();
        }

        
    }
}