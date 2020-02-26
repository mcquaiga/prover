using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Verifications.Dialogs;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public interface IVolumeTestManagerFactory
    {
        //VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
        VolumeTestManager CreateInstance(DeviceInstance deviceManagerDevice, VolumeViewModelBase volumeTest,
            string tachPortName = null);
    }

    public class VolumeTestManagerFactory : IVolumeTestManagerFactory
    {
        private readonly IConfiguration _config;

        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;

        private readonly ILoggerFactory _loggerFactory;
        private readonly Func<OutputChannelType, IOutputChannel> _outputChannelFactory;
        private readonly PulseInputsListenerService _pulseListenerService;

        public VolumeTestManagerFactory(
            ILoggerFactory loggerFactory,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            PulseInputsListenerService pulseListenerService,
            IOutputChannelFactory outputChannelFactory)
            : this(loggerFactory, dialogService, deviceManager, pulseListenerService,
                outputChannelFactory.CreateOutputChannel)
        {
        }

        public VolumeTestManagerFactory(ILoggerFactory loggerFactory,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            PulseInputsListenerService pulseListenerService,
            Func<OutputChannelType, IOutputChannel> outputChannelFactory)
        {
            _loggerFactory = loggerFactory;
            _dialogService = dialogService;
            _pulseListenerService = pulseListenerService;
            _outputChannelFactory = outputChannelFactory;
            _deviceManager = deviceManager;
        }

        public VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest,
            string tachPortName = null)
        {
            var logger = _loggerFactory.CreateLogger(typeof(VolumeTestManager));
            var tachometerService = GetTachometerService(tachPortName, device);
            var pulseInputListener = GetPulseOutputListener(device);
            var motorControl = _outputChannelFactory.Invoke(OutputChannelType.Motor);

            return new VolumeTestManager(logger, _dialogService, _deviceManager, tachometerService,
                pulseInputListener, volumeTest, motorControl);
        }

        private PulseInputsListenerService GetPulseOutputListener(DeviceInstance device)
        {
            _pulseListenerService.Initialize(device.ItemGroup<PulseOutputItems>());
            return _pulseListenerService;
        }

        private ITachometerService GetTachometerService(string portName, DeviceInstance device)
        {
            if (string.IsNullOrEmpty(portName)) return new NullTachometerService();

            var outputChannel = _outputChannelFactory.Invoke(OutputChannelType.Tachometer);
            return new TachometerService(_loggerFactory.CreateLogger(nameof(TachometerService)), portName,
                outputChannel);
        }
    }


    public class VolumeTestManager : ReactiveObject
    {
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IOutputChannel _motorControl;
        protected readonly PulseInputsListenerService PulseListenerService;
        protected readonly ITachometerService TachometerService;
        private CancellationTokenSource _cancellationToken;

        protected IObservable<VolumeTestStatusMessage> TestStatusObservable;

        internal VolumeTestManager(ILogger logger,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseInputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest, 
            IOutputChannel motorControl)
        {
            _logger = logger;
            _dialogService = dialogService;
            _motorControl = motorControl;

            TachometerService = tachometerService;
            PulseListenerService = pulseListenerService;
            VolumeTest = volumeTest;
            DeviceManager = deviceManager;

            StartVolumeTest = ReactiveCommand.CreateFromTask(RunAsync);
        }

        public ReactiveCommand<Unit, Unit> StartVolumeTest { get; protected set; }

        public VolumeViewModelBase VolumeTest { get; protected set; }
        public DeviceSessionManager DeviceManager { get; protected set; }

        public async Task RunAsync()
        {
            _cancellationToken = new CancellationTokenSource();

            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.UpdateStartValues(startValues);

            DisplayDialog();
            var pulses = PulseListenerService.StartListening().Connect()
                .Transform(p => );
            _motorControl.SignalStart();
            
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        protected async Task CompleteAsync()
        {
            _dialogService.CloseDialog.Execute();

            var endValues = await DeviceManager.GetItemValues();
            VolumeTest.UpdateEndValues(endValues);
        }

        protected void DisplayDialog()
        {
            var dialog = new VolumeTestDialogViewModel(TestStatusObservable, _cancellationToken);
            _dialogService.ShowDialog.Execute(dialog);
        }
    }

    public class VolumeTestStatusMessage
    {
        public VolumeTestStatusMessage()
    }
}