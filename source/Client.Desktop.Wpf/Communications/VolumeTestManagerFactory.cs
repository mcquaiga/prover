using System;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Communications
{
    public interface IVolumeTestManagerFactory
    {
        //VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
        RotaryVolumeTestManager CreateInstance(IDeviceSessionManager deviceManager, VolumeViewModelBase volumeTest,
            string tachPortName = null);
    }

    public class VolumeTestManagerFactory : IVolumeTestManagerFactory
    {
        private readonly DialogServiceManager _dialogService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Func<OutputChannelType, IOutputChannel> _outputChannelFactory;
        private readonly PulseOutputsListenerService _pulseListenerService;

        public VolumeTestManagerFactory(
            ILoggerFactory loggerFactory,
            DialogServiceManager dialogService,
            PulseOutputsListenerService pulseListenerService,
            IOutputChannelFactory outputChannelFactory)
            : this(loggerFactory, dialogService, pulseListenerService, outputChannelFactory.CreateOutputChannel)
        {
        }

        public VolumeTestManagerFactory(
            ILoggerFactory loggerFactory,
            DialogServiceManager dialogService,
            PulseOutputsListenerService pulseListenerService,
            Func<OutputChannelType, IOutputChannel> outputChannelFactory)
        {
            _loggerFactory = loggerFactory;
            _dialogService = dialogService;
            _pulseListenerService = pulseListenerService;
            _outputChannelFactory = outputChannelFactory;
        }

        public RotaryVolumeTestManager CreateInstance(IDeviceSessionManager deviceManager, VolumeViewModelBase volumeTest,
            string tachPortName = null)
        {
            var logger = _loggerFactory.CreateLogger<RotaryVolumeTestManager>();

            var tachometerService = GetTachometerService(tachPortName);
            var pulseInputListener = GetPulseOutputListener(deviceManager.Device.ItemGroup<PulseOutputItems>());
            var motorControl = _outputChannelFactory.Invoke(OutputChannelType.Motor);

            return new RotaryVolumeTestManager(logger, deviceManager, tachometerService, pulseInputListener, volumeTest, motorControl);
        }

        private PulseOutputsListenerService GetPulseOutputListener(PulseOutputItems pulseOutputItems)
        {
            _pulseListenerService.Initialize(pulseOutputItems);
            return _pulseListenerService;
        }

        private ITachometerService GetTachometerService(string portName)
        {
            if (string.IsNullOrEmpty(portName)) return new NullTachometerService();

            var outputChannel = _outputChannelFactory.Invoke(OutputChannelType.Tachometer);
            return new TachometerService(_loggerFactory.CreateLogger(nameof(TachometerService)), portName,
                outputChannel);
        }
    }
}