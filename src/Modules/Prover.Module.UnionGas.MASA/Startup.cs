using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Validators.CompanyNumber;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient()).As<DCRWebServiceSoap>();

            //Login service
            builder.RegisterType<LoginService>().As<ILoginService<EmployeeDTO>>().SingleInstance();

            builder.RegisterType<CompanyNumberValidationManager>().As<PreTestValidationBase>();

            builder.RegisterType<ExportManager>().As<IExportTestRun>();
        }
    }
}