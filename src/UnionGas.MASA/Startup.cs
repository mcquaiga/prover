using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using Prover.Core.VerificationTests.TestActions;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Validators.CompanyNumber;
using Module = Autofac.Module;

namespace UnionGas.MASA
{
    public class UnionGasModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient("DCRWebServiceSoap"));

            //Login service
            builder.RegisterType<LoginService>().As<ILoginService<EmployeeDTO>>().SingleInstance();
            builder.RegisterType<CompanyNumberValidationManager>().As<IPreTestValidation>();
            builder.RegisterType<ExportToMasaManager>().As<IExportTestRun>();
           
        }
    }
}