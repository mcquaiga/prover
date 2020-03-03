using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Services;
using Prover.Application.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class NewTestViewModel : RoutableViewModelBase, IRoutableViewModel, IDisposable, IActivatableViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        [Reactive] public TestManager TestManager { get; set; }

        public NewTestViewModel(IScreenManager screenManager,
            ITestManagerViewModelFactory testManagerViewModelFactory,
            DeviceRepository deviceRepository) : base(screenManager)
        {
            
            var canStartTest = this.WhenAnyValue(x => x.SelectedDeviceType, x => x.SelectedCommPort,
                x => x.SelectedBaudRate,
                (device, comm, baud) =>
                    device != null &&
                    !string.IsNullOrEmpty(comm) &&
                    baud != 0);

            StartTestCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                SetLastUsedSettings();
                testManagerViewModelFactory.StartNew(SelectedDeviceType, SelectedCommPort, SelectedBaudRate, SelectedTachCommPort);
                return Observable.Return(Unit.Default);
                //await ScreenManager.ChangeView(TestManager);
            }, canStartTest);

            var canGoForward = this.WhenAnyValue(x => x.TestManager).Select(test => test != null);
            NavigateForward = ReactiveCommand.CreateFromTask(async () => await screenManager.ChangeView(TestManager), canGoForward);

            StartTestCommand.DisposeWith(_cleanup);
            NavigateForward.DisposeWith(_cleanup);

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

        }

        public ReactiveCommand<Unit, IRoutableViewModel> NavigateForward { get; set; }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }

        public ReadOnlyObservableCollection<int> BaudRates { get; set; }

        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        public LocalSettings PreviousSelections => ApplicationSettings.Local;

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        public override string UrlPathSegment => "/VerificationTests";
        public override IScreen HostScreen => ScreenManager;

        public void Dispose()
        {
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