using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class QaTestRunViewModel : RoutableViewModelBase, IDisposable, IDialogViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public QaTestRunViewModel(
            ILogger<QaTestRunViewModel> logger, 
            IScreenManager screenManager,
            IVerificationTestService verificationService,
            IDeviceRepository deviceRepository
            ) : base(screenManager)
        {
            var canStartTest = this.WhenAnyValue(x => x.SelectedDeviceType, x => x.SelectedCommPort,
                x => x.SelectedBaudRate,
                (device, comm, baud) =>
                    device != null &&
                    !string.IsNullOrEmpty(comm) &&
                    baud != 0);

            StartTestCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                SetLastUsedSettings();
                
                TestManager = await verificationService.NewTestManager(SelectedDeviceType);
                
            }, canStartTest);

            deviceRepository.All.Connect()
                .Filter(d => !d.IsHidden)
                .Sort(SortExpressionComparer<DeviceType>.Ascending(p => p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var deviceTypes)
                .Filter(d => d.Id == ApplicationSettings.Local.LastDeviceTypeUsed)
                .Do(d => SelectedDeviceType = d.FirstOrDefault().Current)
                .Subscribe()
                .DisposeWith(_cleanup);
            DeviceTypes = deviceTypes;

            SerialPort.GetPortNames().AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var ports)
                .Subscribe().DisposeWith(_cleanup);
            CommPorts = ports;

            SerialPort.BaudRates.AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var baudRates)
                .Subscribe()
                .DisposeWith(_cleanup);
            BaudRates = baudRates;

            SelectedBaudRate = ApplicationSettings.Local.InstrumentBaudRate;
            SelectedCommPort = ApplicationSettings.Local.InstrumentCommPort;
            SelectedTachCommPort = ApplicationSettings.Local.TachCommPort;

            StartTestCommand.DisposeWith(_cleanup);

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                logger.LogDebug("Saving test...");
                var success = await verificationService.AddOrUpdate(TestManager.TestViewModel);
                if (success)
                {
                    await NotificationInteractions.SnackBarMessage.Handle("SAVED");
                    logger.LogDebug("Saved test successfully");
                }

                return success;
            });

            SubmitTest = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveCommand.Execute();

                (TestManager as IDisposable)?.Dispose();

                await ScreenManager.GoHome();
            });

            PrintTestReport = ReactiveCommand.CreateFromObservable(() =>
                MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented."));
        }

        [Reactive] public ITestManager TestManager { get; private set; }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }
        public ReadOnlyObservableCollection<int> BaudRates { get; set; }
        public ReadOnlyObservableCollection<string> CommPorts { get; set; }
        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        public override string UrlPathSegment => "/VerificationTests";
        public override IScreen HostScreen => ScreenManager;

        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }

        [Reactive] public bool IsDialogOpen { get; private set; }

        public void Dispose()
        { 
            (TestManager as IDisposable)?.Dispose();
            _cleanup?.Dispose();
        }

        private void SetLastUsedSettings()
        {
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;
            ApplicationSettings.Local.InstrumentCommPort = SelectedCommPort;
            ApplicationSettings.Local.InstrumentBaudRate = SelectedBaudRate;
            ApplicationSettings.Local.TachCommPort = SelectedTachCommPort;
            ApplicationSettings.Instance.SaveSettings();
        }
    }
}