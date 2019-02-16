using System.Reflection;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Validators;
using UnionGas.MASA.Validators.CompanyNumber;

namespace UnionGas.MASA
{
    public static class Startup
    {
        public static void Initialize(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient("DCRWebServiceSoap"));
            
            builder.RegisterType<DCRWebServiceCommunicator>();

            //Login service
            builder.RegisterType<LoginService>().As<ILoginService<EmployeeDTO>>()
                .SingleInstance();

            builder.RegisterType<CompanyNumberValidator>().As<IValidator>().AsSelf();
            builder.RegisterType<CompanyNumberUpdater>().As<IUpdater>();
            builder.RegisterType<NewCompanyNumberPopupRequestor>().As<IGetValue>();

            builder.RegisterType<UserLoggedInValidator>().As<IValidator>();

            builder.RegisterType<ExportManager>().As<IExportTestRun>();
            
        }
    }
}