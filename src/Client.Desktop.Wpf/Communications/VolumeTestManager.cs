using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Verifications.Dialogs;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.ExternalDevices;
using Prover.Application.ExternalDevices.DInOutBoards;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public interface IVolumeTestManagerFactory
    {
        //VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest);
        VolumeTestManager CreateInstance(DeviceInstance deviceManagerDevice, VolumeViewModelBase volumeTest, string tachPortName = null);
    }

    public class VolumeTestManagerFactory : IVolumeTestManagerFactory
    {
        private readonly IConfiguration _config;

        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;

        private readonly ILoggerFactory _loggerFactory;
        private readonly IOutputChannelFactory _outputChannelFactory;

        private PulseInputsListenerService _pulseListenerService;

        public VolumeTestManagerFactory(ILoggerFactory loggerFactory,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            PulseInputsListenerService pulseListenerService,
            IOutputChannelFactory outputChannelFactory)
        {
            _loggerFactory = loggerFactory;
            _dialogService = dialogService;
            _pulseListenerService = pulseListenerService;
            _outputChannelFactory = outputChannelFactory;
            _deviceManager = deviceManager;
        }

        public VolumeTestManager CreateInstance(DeviceInstance device, VolumeViewModelBase volumeTest, string tachPortName = null)
        {
            var logger = _loggerFactory.CreateLogger(typeof(VolumeTestManager));

            var tachometerService = GetTachometerService(tachPortName);

            return new VolumeTestManager(logger, _dialogService, _deviceManager, tachometerService, _pulseListenerService,
                volumeTest);
        }

        private ITachometerService GetTachometerService(string portName)
        {
            if (string.IsNullOrEmpty(portName))
            {
                return new NullTachometerService();
            }

            var outputChannel = _outputChannelFactory.CreateOutputChannel(OutputChannelType.Tachometer);
            return new TachometerService(_loggerFactory.CreateLogger(nameof(TachometerService)), portName, outputChannel);
        }
    }


    public class VolumeTestManager : ReactiveObject
    {
        private readonly ILogger _logger;
        private readonly DialogServiceManager _dialogService;
        protected readonly PulseInputsListenerService PulseListenerService;
        protected readonly ITachometerService TachometerService;
        private CancellationTokenSource _cancellationToken;

        protected IObservable<VolumeTestStatusMessage> TestStatusObservable;

        internal VolumeTestManager(
            ILogger logger,
            DialogServiceManager dialogService,
            DeviceSessionManager deviceManager,
            ITachometerService tachometerService,
            PulseInputsListenerService pulseListenerService,
            VolumeViewModelBase volumeTest            )
        {
            _logger = logger;
            _dialogService = dialogService;
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

            SetupIODevices();

            var startValues = await DeviceManager.GetItemValues();
            VolumeTest.UpdateStartValues(startValues);

            DisplayDialog();

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

        private void SetupIODevices()
        {
            //Setup DAQ board
            //Setup Tachometer
        }
    }

    public class VolumeTestStatusMessage
    {
    }
}