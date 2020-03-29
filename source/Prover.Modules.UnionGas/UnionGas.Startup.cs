using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interfaces;
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
                screen => screen.ChangeView<ExporterViewModel>(), 4);
    }

    public class Startup : IConfigureModule
    {
        public void Configure(HostBuilderContext builder, IServiceCollection services)
        {
            services.AddSingleton(c => MainMenuItems.ExporterMainMenu);
            services.AddSingleton<IToolbarItem, LoginToolbarViewModel>();

            services.AddViewsAndViewModels();

            AddDCRWebService(services);

            if (builder.HostingEnvironment.IsDevelopment())
                services.AddSingleton<ILoginService<EmployeeDTO>, LocalLoginService>();
            else
                services.AddSingleton<ILoginService<EmployeeDTO>, MasaLoginService>();

            services.AddScoped<IExportVerificationTest, ExportToMasaManager>();
        }

        private void AddDCRWebService(IServiceCollection services)
        {
            services.AddSingleton<DCRWebServiceSoap>(c =>
                new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap));

            services.AddSingleton<DCRWebServiceCommunicator>();
        }
    }
}

//services.AddSingleton<Func<DeviceInstance, UnionGasEvcVerification>>(c =>
//    device => new UnionGasEvcVerification(device, c.GetService<ILoginService<EmployeeDTO>>().User?.Id));

//services.AddSingleton<IAsyncRepository<UnionGasEvcVerification>>(c => new LiteDbAsyncRepository<UnionGasEvcVerification>(c.GetService<ILiteDatabase>()));
//services.AddSingleton(c => new EvcVerificationTestService<UnionGasEvcVerification>(c.GetService<IAsyncRepository<UnionGasEvcVerification>>()));

////services.AddSingleton(c => (Func<UnionGasEvcVerificationViewModel, VerificationTestService<UnionGasEvcVerification, UnionGasEvcVerificationViewModel>, ITestManager>)
////    c.GetService<Func<EvcVerificationViewModel, VerificationTestService<EvcVerificationTest, EvcVerificationViewModel>, ITestManager>>());

//services.AddSingleton<Func<UnionGasEvcVerificationViewModel, VerificationTestService<UnionGasEvcVerification, UnionGasEvcVerificationViewModel>, ITestManager>>(c =>
//            (test, service) => new TestManager(
//                c.GetService<ILogger<TestManager>>(),
//                c.GetService<IDeviceSessionManager>(),
//                test,
//                c.GetService<Func<EvcVerificationViewModel, IVolumeTestManager>>())
//);
//services.AddSingleton(c => (Func<UnionGasEvcVerificationViewModel, IVolumeTestManager>)
//    c.GetService<Func<EvcVerificationViewModel, IVolumeTestManager>>());

//services.AddSingleton<VerificationTestService<UnionGasEvcVerification, UnionGasEvcVerificationViewModel>>();

//services.Replace(ServiceDescriptor.Singleton(c => (VerificationTestService<IEvcVerificationTest, EvcVerificationViewModel>) 
//    c.GetService<VerificationTestService<UnionGasEvcVerification, UnionGasEvcVerificationViewModel>>()));


//        builder.RegisterType<CompanyNumberValidationManager>()
//            .As<IEvcDeviceValidationAction>()
//            .AsSelf();

//        builder.RegisterType<UserLoggedInValidator>().As<IValidator>();