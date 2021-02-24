using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.VerificationTests.TestActions;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Validators;
using UnionGas.MASA.Validators.CompanyNumber;

namespace UnionGas.MASA {
	public class UnionGasModule : Module {
		protected override void Load(ContainerBuilder builder) {
			builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient("DCRWebServiceSoap"));

			builder.RegisterType<DCRWebServiceCommunicator>()
				.SingleInstance();

			//Login service
			builder.RegisterType<LoginService>()
				.As<ILoginService<EmployeeDTO>>()
				.SingleInstance();


			builder.RegisterType<ExportToMasaManager>().As<IExportTestRun>();

			builder.RegisterType<CompanyNumberValidationManager>()
				.As<IEvcDeviceValidationAction>()
				.AsSelf();

			builder.RegisterType<UserLoggedInValidator>().As<IValidator>();
		}
	}
}