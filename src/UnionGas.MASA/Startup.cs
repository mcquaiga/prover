using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using Prover.GUI.Common.Screens.MainMenu;
using ReactiveUI.Autofac;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //App Menu Icon
            builder.RegisterType<ExportManagerApp>().Named<IAppMainMenu>("ExportApp");

            builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient());

            builder.RegisterType<CompanyNumberUpdater>().Named<IUpdater>("CompanyNumberUpdater");
            builder.RegisterType<CompanyNumberVerifier>().Named<IVerifier>("CompanyNumberVerifier");
            builder.RegisterType<ExportManager>().As<IExportTestRun>();

            //ViewModels
            builder.RegisterViewModels(assembly);
            builder.RegisterViews(assembly);
            builder.RegisterScreen(assembly);

            //Login service
            builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
        }
    }
}