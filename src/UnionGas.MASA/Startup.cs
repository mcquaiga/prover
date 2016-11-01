using System;
using System.Reflection;
using Autofac;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using ReactiveUI.Autofac;
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