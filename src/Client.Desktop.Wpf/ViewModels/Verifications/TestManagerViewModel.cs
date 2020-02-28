using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Interactions;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Extensions;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public interface ITestManagerViewModelFactory
    {
        Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName);
    }

    //public class TestManagerViewModelFactory : ITestManagerViewModelFactory
    //{
    //    private readonly DeviceSessionManager _deviceManager;
    //    private readonly DialogServiceManager _dialogService;
    //    private readonly ILoggerFactory _loggerFactory;
    //    private readonly IScreenManager _screenManager;
    //    private readonly VerificationViewModelService _testViewModelService;
    //    private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;

    //    public TestManagerViewModelFactory(
    //        ILoggerFactory loggerFactory,
    //        IScreenManager screenManager,
    //        DeviceSessionManager deviceManager,
    //        VerificationViewModelService testViewModelService,
    //        DialogServiceManager dialogService,
    //        IVolumeTestManagerFactory volumeTestManagerFactory)
    //    {
    //        _loggerFactory = loggerFactory;
    //        _screenManager = screenManager;
    //        _deviceManager = deviceManager;
    //        _testViewModelService = testViewModelService;
    //        _dialogService = dialogService;
    //        _volumeTestManagerFactory = volumeTestManagerFactory;
    //    }

    //    public async Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
    //        string tachPortName)
    //    {
    //        await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);
    //        var testViewModel = _testViewModelService.NewTest(_deviceManager.Device);
    //        var volumeTestManager =
    //            _volumeTestManagerFactory.CreateInstance(_deviceManager.Device, testViewModel.VolumeTest);

    //        return new TestManagerViewModel(_loggerFactory.CreateLogger(nameof(TestManagerViewModel)), _screenManager, _deviceManager, _testViewModelService,
    //            _dialogService, testViewModel, volumeTestManager);
    //    }
    //}

    public partial class TestManagerViewModel : RoutableViewModelBase, IRoutableViewModel, IDisposable
    {
        private readonly DeviceSessionManager _deviceManager;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger _logger;
        private readonly IScreenManager _screenManager;
        private readonly VerificationViewModelService _testViewModelService;
        private readonly IVolumeTestManagerFactory _volumeTestManagerFactory;
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        [Reactive] public EvcVerificationViewModel TestViewModel { get; protected set; }

        [Reactive] public VolumeTestManager VolumeTestManager { get; protected set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> CompleteTest { get; protected set; }

        public ReactiveCommand<Unit, Unit> RunVolumeTest { get; protected set; }

        public override string UrlPathSegment => "/VerificationTests/Details";
        public override IScreen HostScreen => _screenManager;

        public async Task Complete()
        {
            //await SaveCurrentState();
        }

        public void Dispose()
        {
            _logger.LogDebug($"Disposing instance created at {_dateCreated}." );
            _cleanup?.Dispose();
        }

        public async Task DownloadItems(VerificationTestPointViewModel test)
        {
            var toDownload = GetItemsToDownload(_deviceManager.Device.Composition());

            var values = await _deviceManager.DownloadCorrectionItems(toDownload);

            test.UpdateItemValues(values);

            foreach (var correction in test.GetCorrectionTests())
            {
                var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));
                itemType?.SetValue(correction,
                    _deviceManager.DeviceType.GetGroupValues(values, itemType.PropertyType));
            }
        }

        private ICollection<ItemMetadata> GetItemsToDownload(CompositionType compType)
        {
            var items = new List<ItemMetadata>();

            if (compType == CompositionType.P || compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<PressureItems>());

            if (compType == CompositionType.T || compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<TemperatureItems>());

            if (compType == CompositionType.PTZ)
                items.AddRange(_deviceManager.DeviceType.GetItemMetadata<SuperFactorItems>());

            return items;
        }

        private void RunNavigatingAwayActions()
        {
            
            // Ask if they want to save
        }

        private void SetupRxUi()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var success = await _testViewModelService.AddOrUpdate(TestViewModel);
                if (success) await NotificationInteractions.SnackBarMessage.Handle("Saved Successfully!");
                return success;
            });
            SaveCommand.ThrownExceptions.LogErrors("Error saving test.").Subscribe();

            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(DownloadItems);
            DownloadCommand.ThrownExceptions.LogErrors("Error downloading items from instrument.").Subscribe();

            PrintTestReport = ReactiveCommand.CreateFromObservable(() => MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented."));

            RunVolumeTest = ReactiveCommand.CreateFromTask(VolumeTestManager.RunAsync);

            CompleteTest = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveCommand.Execute();
                await _deviceManager.EndSession();
            });

            SetupAutoSave();

            SaveCommand.DisposeWith(_cleanup);
            DownloadCommand.DisposeWith(_cleanup);
            PrintTestReport.DisposeWith(_cleanup);
            RunVolumeTest.DisposeWith(_cleanup);
        }

        private void SetupAutoSave()
        {
            this.WhenAnyValue(x => x.TestViewModel);
        }

        async Task Reset()
        {
            await _deviceManager.EndSession();
        }
    }

    public partial class TestManagerViewModel : ITestManagerViewModelFactory
    {
        private readonly DateTime _dateCreated;

        public TestManagerViewModel(ILoggerFactory loggerFactory,
            IScreenManager screenManager,
            DeviceSessionManager deviceManager,
            VerificationViewModelService testViewModelService,
            DialogServiceManager dialogService,
            IVolumeTestManagerFactory volumeTestManagerFactory) : base(screenManager)
        {
            _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _screenManager = screenManager;
            _deviceManager = deviceManager;
            _testViewModelService = testViewModelService;
            _dialogService = dialogService;
            _volumeTestManagerFactory = volumeTestManagerFactory;

            _dateCreated = DateTime.Now;

            this.WhenNavigatingFromObservable()
                .Subscribe(async _ =>
                {
                    _logger.LogDebug($"Navigating away from Cleaning up.");
                    await _deviceManager.EndSession();
                    RunNavigatingAwayActions();
                })
                .DisposeWith(_cleanup);

           
        }

        public async Task<TestManagerViewModel> StartNew(DeviceType deviceType, string commPortName, int baudRate,
            string tachPortName)
        {
            if (_deviceManager.SessionInProgress) await Reset();

            await _deviceManager.StartSession(deviceType, commPortName, baudRate, null);
            
            TestViewModel = _testViewModelService.NewTest(_deviceManager.Device);
            TestViewModel.DisposeWith(_cleanup);

            VolumeTestManager = _volumeTestManagerFactory.CreateInstance(_deviceManager, TestViewModel.VolumeTest, tachPortName);
            VolumeTestManager.DisposeWith(_cleanup);

            SetupRxUi();

            await _screenManager.ChangeView(this);
            
            return this;
        }
    }

    //public static class MyExtensions
    //{
    //    public static IObservable<Exception> LogErrors(this IObservable<Exception> source, string message = null, ILogger logger = null)
    //    {
    //        var time = DateTime.Now;

    //        if (logger != null)
    //            return source.Do(ex => logger.LogError(ex, $"{time} - {message}."));

    //        return source.Do(ex =>
    //        {
    //            Debug.WriteLine($"{time} - {message}. {Environment.NewLine}" +
    //                            $"Exception: {ex}");
    //        });
    //    }

    //    public static IObservable<T> LogErrors<T>(this IObservable<T> source, ILogger logger)
    //    {
    //        var time = DateTime.Now;
    //        return source.Do(changes => { }, ex => logger.LogError(ex, $"{time} - Error on {source} of {typeof(T)}."));
    //    }

    //    public static IObservable<T> LogDebug<T>(this IObservable<T> source, string message)
    //    {
    //        var time = DateTime.Now;
    //        return source.Do(x => Debug.WriteLine($"{time} - {message}"));
    //    }
    //}
}