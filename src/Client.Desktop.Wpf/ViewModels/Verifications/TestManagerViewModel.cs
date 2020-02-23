using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Application.Services;
using Application.Settings;
using Application.ViewModels;
using Application.ViewModels.Volume;
using Client.Wpf.Communications;
using Devices;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Domain.EvcVerifications;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Interfaces;

namespace Client.Desktop.Wpf.ViewModels.Verifications
{
    public class TestManagerViewModel : ViewModelBase, IRoutableViewModel
    {
        public enum TestManagerState
        {
            Start,
            InProgress
        }

        public TestManagerViewModel(IScreenManager screenManager, VerificationTestManager testManager) : base(screenManager)
        {
            TestManager = testManager;

            var canStartTest = this.WhenAnyValue(x => x.SelectedDeviceType, x => x.SelectedCommPort, x => x.SelectedBaudRate,
                (device, comm, baud) => device != null && !string.IsNullOrEmpty(comm) && baud != 0);

            StartTestCommand = ReactiveCommand.CreateFromTask(StartTest, canStartTest);
            StartTestCommand.Subscribe(_ => SetLastUsedSettings());

            StartTestCommand
                .Select(x => TestManagerState.InProgress)
                .ToPropertyEx(this, x => x.TestManagerScreenState, TestManagerState.Start);

            SetupNewTestView();
        }

        public LocalSettings Settings => ApplicationSettings.Local;

        public extern TestManagerState TestManagerScreenState { [ObservableAsProperty] get; }

        public VerificationTestManager TestManager { get; protected set; }

        [Reactive] public VolumeViewModelBase VolumeViewModel { get; set; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }

        //[Reactive] public EvcVerificationViewModel EvcVerification { get; set; }

        public string UrlPathSegment => "/VerificationTests/Details";
        public IScreen HostScreen => ScreenManager;

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }

        public ReadOnlyObservableCollection<int> BaudRates { get; set; }

        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        [Reactive] public string SelectedTachCommPort { get; set; }
        public string SelectedCommPort
        {
            get => ApplicationSettings.Local.InstrumentCommPort;
            set => ApplicationSettings.Local.InstrumentCommPort = value;
        }

        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        private void SetLastUsedSettings()
        {
            ApplicationSettings.Local.LastDeviceTypeUsed = SelectedDeviceType.Id;
            ApplicationSettings.Local.InstrumentCommPort = SelectedCommPort;
            ApplicationSettings.Local.InstrumentBaudRate = SelectedBaudRate;
            ApplicationSettings.Instance.SaveSettings();
        }

        private void SetupNewTestView()
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
        }

        private async Task StartTest()
        {
            SetLastUsedSettings();

            await TestManager.StartNew(SelectedDeviceType, SelectedCommPort, SelectedBaudRate, "COM3");

            SetupTestManagerView();
        }

        private void SetupTestManagerView()
        {
            //EvcVerification = TestManager.TestViewModel;
            SaveCommand = ReactiveCommand.CreateFromTask(() => TestManager.SaveCurrentState());
            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(test => TestManager.DownloadItems(test));
        }
    }
}