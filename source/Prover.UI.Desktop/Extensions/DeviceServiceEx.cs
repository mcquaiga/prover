using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.Verifications;
using Prover.Application.Verifications.Corrections;
using Prover.Application.Verifications.Factories;
using Prover.Application.Verifications.Volume;
using Prover.Application.ViewModels;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Communications;
using Prover.UI.Desktop.ViewModels.Verifications;
using System;
using Microsoft.Extensions.Configuration;

namespace Prover.UI.Desktop.Extensions
{
	internal static class DeviceServiceEx
	{
		public static void AddDeviceCommunication(this IServiceCollection services)
		{
			services.AddSingleton<IDeviceSessionManager, DeviceSessionManager>();

			// Port Setup
			var portFactory = new CommPortFactory();
			var clientFactory = new CommunicationsClientFactory(portFactory);

			// Device Clients
			services.AddSingleton<Func<DeviceType, ICommunicationsClient>>(c => (device) =>
			{
				var port = portFactory.Create(
					ApplicationSettings.Local.InstrumentCommPort,
					ApplicationSettings.Local.InstrumentBaudRate);

				return clientFactory.Create(device, port);
			});
		}

		public static void AddPulseOutputListeners(this IServiceCollection services)
		{
			services.AddTransient<PulseOutputsListenerService>();
			services.AddSingleton<Func<PulseOutputsListenerService>>(c => c.GetService<PulseOutputsListenerService>);

			//services.AddSingleton<DaqBoardChannelFactory>();
			//services.AddSingleton<IInputChannelFactory, DaqBoardChannelFactory>();
			//services.AddSingleton<IOutputChannelFactory, DaqBoardChannelFactory>();

			// Pulse Outputs
			// Simulator
			//services.AddSingleton(c => new SimulatorPulseChannelFactory(c.GetService<ILogger>()));
			//services.AddSingleton<IInputChannelFactory, SimulatorPulseChannelFactory>();
			//services.AddSingleton<IOutputChannelFactory, SimulatorPulseChannelFactory>();
		}

		public static void AddVerificationManagers(this IServiceCollection services)
		{

			services.AddTransient<Func<EvcVerificationViewModel, IVolumeTestManager>>(c => evcTest =>
			{
				var volumeFactory = c.GetService<IVolumeTestManagerFactory>();
				return volumeFactory.CreateVolumeManager(evcTest);
			});

			services.AddTransient<Func<EvcVerificationViewModel, IQaTestRunManager>>(c =>
			{
				return test =>
				{
					var volumeManager = c.GetService<IVolumeTestManagerFactory>()
										 .CreateVolumeManager(test);

					var testManager = ActivatorUtilities.CreateInstance<TestManager>(c, test, volumeManager, c.GetService<ICorrectionTestsManager>());

					return testManager;
				};
			});
			services.AddTransient<ICorrectionTestsManager, LiveReadStabilizeCorrectionTestManager>();
			services.AddTransient<IVerificationManagerService, VerificationManagerService>();
			services.AddTransient<IVolumeTestManagerFactory>(c =>
			{
				//				if (ApplicationSettings.Shared.TestSettings.)
				return ActivatorUtilities.CreateInstance<AutomatedVolumeTestManagerFactory>(c);
			});
			//services.AddTransient<EvcVerificationViewModel>();
			//services.AddTransient<IVolumeTestManager, RotaryVolumeTestRunner>();
		}

		public static void AddTachometer(this IServiceCollection services)
		{
			services.AddSingleton<Func<IAppliedInputVolume>>(c =>
				() =>
				{
					if (ApplicationSettings.Local.TachIsNotUsed)
						return new NullTachometerService();

					return new TachometerService(
						c.GetService<ILogger<TachometerService>>(),
						ApplicationSettings.Local.TachCommPort,
						c.GetService<Func<OutputChannelType, IOutputChannel>>().Invoke(OutputChannelType.Tachometer)
					);
				});
		}
	}
}