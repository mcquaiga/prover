using System;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.Shared.Interfaces;

namespace Prover.Application.VerificationManager.Volume
{
    public interface IVolumeTestManagerFactory
    {
        //VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
        IVolumeTestManager CreateVolumeManager(EvcVerificationViewModel verificationTest);
    }

    public class VolumeTestManagerFactory : IVolumeTestManagerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly Func<OutputChannelType, IOutputChannel> _outputChannelFactory;
        private readonly Func<PulseOutputsListenerService> _pulseOutputServiceFactory;
        private readonly Func<IAppliedInputVolume> _tachometerServiceFactory;

        public VolumeTestManagerFactory(
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
            var logger = _loggerFactory.CreateLogger<RotaryVolumeManager>();

            var tachometerService = _tachometerServiceFactory.Invoke();
            var pulseOutputListener = GetPulseOutputListener(_deviceManager.Device.ItemGroup<PulseOutputItems>());
            var motorControl = _outputChannelFactory.Invoke(OutputChannelType.Motor);

            switch (verificationTest.VolumeTest)
            {
                case RotaryVolumeViewModel rotary:
                    return new RotaryVolumeManager(logger, _deviceManager, tachometerService, pulseOutputListener, motorControl, rotary);
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
            if (string.IsNullOrEmpty(portName)) return new NullTachometerService();
            return _tachometerServiceFactory.Invoke();
            //var outputChannel = _outputChannelFactory.Invoke(OutputChannelType.Tachometer);
            //return new TachometerService(_loggerFactory.CreateLogger(nameof(TachometerService)), portName,
            //    outputChannel);
        }
    }
}