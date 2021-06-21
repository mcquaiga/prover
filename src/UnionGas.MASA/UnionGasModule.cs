using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.VerificationTests.TestActions;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;
using UnionGas.MASA.Validators;

namespace UnionGas.MASA {

	public class UnionGasModule : Module {

		#region Methods

		protected override void Load(ContainerBuilder builder) {
			//builder.RegisterInstance<DCRWebServiceSoap>(new DCRWebServiceSoapClient("DCRWebServiceSoap"));
			builder.Register(c => {
				var proxy = new DCRWebServiceSoapClient("DCRWebServiceSoap");

				//proxy.ClientCredentials.UserName.UserName = Properties.Settings.Default.Username;
				//proxy.ClientCredentials.UserName.Password = Properties.Settings.Default.Password;
				//proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

				return proxy;
			}).As<DCRWebServiceSoap>();

			builder.RegisterType<DCRWebServiceCommunicator>()
				.SingleInstance();

			//Login service
			builder.RegisterType<LoginService>()
				.As<ILoginService<EmployeeDTO>>()
				.SingleInstance();

			builder.RegisterType<ExportToMasaManager>().As<IExportTestRun>();

			builder.RegisterType<BarCodeValidationManager>()
				.As<IEvcDeviceValidationAction>()
				.AsSelf();

			builder.RegisterType<UserLoggedInValidator>().As<IValidator>();
		}

		#endregion
	}
}