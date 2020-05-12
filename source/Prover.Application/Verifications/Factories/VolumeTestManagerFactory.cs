using System;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.Verifications.Volume;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Prover.Application.Verifications.Factories
{
	public interface IVolumeTestManagerFactory
	{
		//VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
		IVolumeTestManager CreateVolumeManager(EvcVerificationViewModel verificationTest);
	}

	//public static class VolumeTestFactory
	//{
	//	public static IVolumeTestManagerFactory GetFactoryByDriveType(VolumeInputType inputType)
	//	{
	//		switch (inputType)
	//		{
	//			case VolumeInputType.Mechanical:

	//				break;
	//			case VolumeInputType.PulseInput:
	//				break;
	//			case VolumeInputType.Rotary:
	//				break;
	//			default:
	//				throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null);
	//		}
	//	}
	//}

	//public class VolumeTestManagerFactory : IVolumeTestManagerFactory
	//{
	//	public VolumeTestManagerFactory(RotaryVolumeTestManagerFactory rotaryFactory)

	//	/// <inheritdoc />
	//	public IVolumeTestManager CreateVolumeManager(EvcVerificationViewModel verificationTest) => throw new NotImplementedException();
	//}


	public class AutomatedVolumeTestManagerFactory : IVolumeTestManagerFactory
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly IDeviceSessionManager _deviceManager;
		private readonly Func<OutputChannelType, IOutputChannel> _outputChannelFactory;
		private readonly Func<PulseOutputsListenerService> _pulseOutputServiceFactory;
		private readonly Func<IAppliedInputVolume> _tachometerServiceFactory;

		public AutomatedVolumeTestManagerFactory(
			ILoggerFactory loggerFactory,
			IDeviceSessionManager deviceManager,
			Func<PulseOutputsListenerService> pulseOutputServiceFactory,
			Func<OutputChannelType, IOutputChannel> outputChannelFactory,
			Func<IAppliedInputVolume> tachometerServiceFactory = null)
		{
			_loggerFactory = loggerFactory;
			_deviceManager = deviceManager;

			_pulseOutputServiceFactory = pulseOutputServiceFactory;
			_outputChannelFactory = outputChannelFactory;
			_tachometerServiceFactory = tachometerServiceFactory;
		}

		public IVolumeTestManager CreateVolumeManager(EvcVerificationViewModel verificationTest)
		{
			var tachometerService = _tachometerServiceFactory.Invoke();
			var pulseOutputListener = GetPulseOutputListener(verificationTest.Device.ItemGroup<PulseOutputItems>());
			var motorControl = _outputChannelFactory.Invoke(OutputChannelType.Motor);

			switch (verificationTest.VolumeTest)
			{
				case RotaryVolumeViewModel rotary:
				{
					var logger = _loggerFactory.CreateLogger<RotaryVolumeTestRunner>();
					return new RotaryVolumeTestRunner(logger, _deviceManager, tachometerService, pulseOutputListener, motorControl, rotary);
				}

			}

			throw new NotImplementedException("Missing VolumeManager implementation");
		}

		private PulseOutputsListenerService GetPulseOutputListener(PulseOutputItems pulseOutputItems)
		{
			var pulseService = _pulseOutputServiceFactory.Invoke();
			pulseService.Initialize(pulseOutputItems);
			return pulseService;
		}

		private IAppliedInputVolume GetTachometerService(string portName)
		{
			if (string.IsNullOrEmpty(portName))
				return new NullTachometerService();
			return _tachometerServiceFactory.Invoke();
			//var outputChannel = _outputChannelFactory.Invoke(OutputChannelType.Tachometer);
			//return new TachometerService(_loggerFactory.CreateLogger(nameof(TachometerService)), portName,
			//    outputChannel);
		}
	}
}