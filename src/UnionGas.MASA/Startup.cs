using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Common.Screens.Toolbar;
using ReactiveUI.Autofac;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Screens.Toolbars;
using UnionGas.MASA.Validators.CompanyNumber;
using Module = Autofac.Module;

namespace UnionGas.MASA
{
    public class UnionGasModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient()).As<DCRWebServiceSoap>();

            //Login service
            builder.RegisterType<LoginService>().As<ILoginService<EmployeeDTO>>().SingleInstance();

            builder.RegisterType<CompanyNumberValidationManager>().As<IPreTestValidation>();

            builder.RegisterType<ExportManager>().As<IExportTestRun>();
           
        }
    }
}