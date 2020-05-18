using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber;
using Prover.Modules.UnionGas.Login;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Modules.UnionGas.Models;
using Prover.Modules.UnionGas.Verifications;
using Prover.Shared;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using Prover.Storage.LiteDb;
using Prover.UI.Desktop.Controls;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.ViewModels;

namespace Prover.Modules.UnionGas
{
	//public class ExporterMainMenu : IMainMenuItem
	//{
	//	public ExporterMainMenu(IScreenManager screenManager)
	//	{
	//		OpenCommand = ReactiveCommand.CreateFromTask(async () => { await screenManager.ChangeView<ExporterViewModel>(); });
	//	}

	//	public PackIconKind MenuIconKind { get; } = PackIconKind.CloudUpload;
	//	public string MenuTitle { get; } = "Export Test Run";
	//	public ReactiveCommand<Unit, Unit> OpenCommand { get; }
	//	public int? Order { get; } = 2;
	//}

	public class UnionGasModule : IConfigureModule
	{
		/// <inheritdoc />
		public void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config)
		{
			config.AddJsonFile("appsettings.UnionGas.json");
		}



		/// <inheritdoc />
		public void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
		{
			AddServices(builder, services);
		}


		private void AddServices(HostBuilderContext builder, IServiceCollection services)
		{
			services.AddSingleton<IToolbarItem, ExporterViewModel>();
			services.AddSingleton<IToolbarItem, LoginToolbarViewModel>();
			services.AddSingleton<ExportToolbarViewModel>();
			services.AddViewsAndViewModels();

			//services.AddSingleton<Func<EvcVerificationTest, ExporterViewModel, VerificationGridViewModel>>(c => (evcTest, exporter)
			//		=> new VerificationGridViewModel(c.GetService<ILogger<VerificationGridViewModel>>(), evcTest, c.GetService<IVerificationTestService>(), c.GetService<ILoginService<Employee>>(),
			//		c.GetService<IExportVerificationTest>(), exporter));
			services.AddSingleton<MasaLoginService>();
			services.AddSingleton<ILoginService<Employee>>(c => c.GetRequiredService<MasaLoginService>());
			services.AddSingleton<ILoginService>(c => c.GetRequiredService<MasaLoginService>());

			//services.AddHostedService<MasaLoginService>();
			services.AddSingleton<IExportVerificationTest, ExportToMasaManager>();
			services.AddTransient<TestsByJobNumberViewModel>();
			services.AddSingleton<IRepository<Employee>, LiteDbRepository<Employee>>();
			services.AddVerificationActions();

			if (builder.Configuration.UseMasa())
				services.AddMasaWebService(builder.Configuration.MasaRemoteAddress());
			else
			{
				DevelopmentServices(services);
			}
		}

		private void DevelopmentServices(IServiceCollection services)
		{
			services.AddSingleton<DevelopmentWebService>();
			services.AddSingleton<IUserService<EmployeeDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
			services.AddSingleton<IMeterService<MeterDTO>>(c => c.GetRequiredService<DevelopmentWebService>());
			services.AddSingleton<IExportService<QARunEvcTestResult>>(c => c.GetRequiredService<DevelopmentWebService>());
		}
	}

	internal static class ConfigurationEx
	{
		private static readonly string UseMasaKey = "UseMasaWebService";
		private static readonly string MasaAddressKey = "MasaRemoteAddress";
		public static string MasaRemoteAddress(this IConfiguration config) => config.GetValue<string>(MasaAddressKey);


		public static bool UseMasa(this IConfiguration config) => config.GetValue<bool?>(UseMasaKey) ?? false;
	}

	internal static class ServicesEx
	{
		public static void AddMasaWebService(this IServiceCollection services, string remoteAddress = null)
		{
			services.AddSingleton<DCRWebServiceSoap>(c
					=> string.IsNullOrEmpty(remoteAddress)
							? new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap)
							: new DCRWebServiceSoapClient(DCRWebServiceSoapClient.EndpointConfiguration.DCRWebServiceSoap, remoteAddress));
			services.AddSingleton<MasaWebService.MasaWebService>();
			services.AddSingleton<IUserService<EmployeeDTO>>(c => c.GetRequiredService<MasaWebService.MasaWebService>());
			services.AddSingleton<IMeterService<MeterDTO>>(c => c.GetRequiredService<MasaWebService.MasaWebService>());
			services.AddSingleton<IExportService<QARunEvcTestResult>>(c => c.GetRequiredService<MasaWebService.MasaWebService>());
		}

		public static void AddVerificationActions(this IServiceCollection services)
		{
			services.AddSingleton<MeterInventoryNumberValidator>();
			services.AddAllTypes<IEventsSubscriber>(lifetime: ServiceLifetime.Singleton);
		}
	}
}