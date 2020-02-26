using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Client.Desktop.Wpf.Communications;
using Devices;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class NewTestViewModel : ViewModelBase, IRoutableViewModel, IDisposable
    {
        private CompositeDisposable _cleanup;

        public NewTestViewModel(IScreenManager screenManager,
            ITestManagerViewModelFactory testManagerViewModelFactory) : base(screenManager)
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
                var testManager = await testManagerViewModelFactory.StartNew(SelectedDeviceType, SelectedCommPort, SelectedBaudRate, SelectedTachCommPort);
                await ScreenManager.ChangeView(testManager);
            }, canStartTest);


            SetupScreenData();
        }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }

        public ReadOnlyObservableCollection<int> BaudRates { get; set; }

        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        public string UrlPathSegment => "/VerificationTests";
        public IScreen HostScreen => ScreenManager;

        public void Dispose()
        {
            StartTestCommand?.Dispose();
            _cleanup?.Dispose();
        }

        private void SetLastUsedSettings()
        {
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;
            ApplicationSettings.Local.InstrumentCommPort = SelectedCommPort;
            ApplicationSettings.Local.InstrumentBaudRate = SelectedBaudRate;
            ApplicationSettings.Instance.SaveSettings();
        }

        private void SetupScreenData()
        {
            var devTypes = RepositoryFactory.Instance.Connect()
                .Filter(d => !d.IsHidden)
                .Sort(SortExpressionComparer<DeviceType>.Ascending(p => p.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var deviceTypes)
                .DisposeMany()
                .Filter(d => d.Id == ApplicationSettings.Local.LastDeviceTypeUsed)
                .Select(d => d.FirstOrDefault().Current)
                .Do(d => SelectedDeviceType = d)
                .Subscribe();

            DeviceTypes = deviceTypes;

            SerialPort.GetPortNames().AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var ports)
                .Subscribe();
            CommPorts = ports;

            SelectedCommPort = ApplicationSettings.Local.InstrumentCommPort;

            SerialPort.BaudRates.AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var baudRates)
                .Subscribe();
            BaudRates = baudRates;

            SelectedBaudRate = ApplicationSettings.Local.InstrumentBaudRate;

            _cleanup = new CompositeDisposable(devTypes);
        }

    }
}