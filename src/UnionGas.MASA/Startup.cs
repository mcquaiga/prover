using System;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;
using UnionGas.MASA.Dialogs.LoginDialog;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Screens.Exporter;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(IUnityContainer container)
        {
            //App Menu Icon
            container.RegisterType<IAppMainMenu, ExportManagerApp>("ExportApp");

            container.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient());

            container.RegisterType<IUpdater, CompanyNumberUpdater>();
            container.RegisterType<IVerifier, CompanyNumberVerifier>();
            container.RegisterType<IExportTestRun, ExportManager>();

            //ViewModels
            container.RegisterType<ExportTestsViewModel>();
            container.RegisterType<QaTestRunGridViewModel>();
            container.RegisterType<LoginDialogViewModel>();
            container.RegisterType<CompanyNumberDialogViewModel>();

            //Login service
            var loginService = new LoginService(container.Resolve<ScreenManager>(), container.Resolve<IEventAggregator>(), container.Resolve<DCRWebServiceSoap>());
            container.RegisterInstance<ILoginService>(loginService, new ContainerControlledLifetimeManager());
        }
    }
}