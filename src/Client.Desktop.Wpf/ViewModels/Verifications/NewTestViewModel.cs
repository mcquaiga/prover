using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Application.Services;
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

namespace Client.Wpf.ViewModels.Verifications
{
    public class NewTestViewModel : ViewModelBase, IRoutableViewModel, IDisposable
    {
        private readonly VerificationTestManager _testManager;
        private readonly IAsyncRepository<EvcVerificationTest> _repository;
        private readonly VerificationViewModelService _service;


        public NewTestViewModel(IScreenManager screenManager, 
            VerificationTestManager testManager, 
            IAsyncRepository<EvcVerificationTest> repository,
            VerificationViewModelService service) : base(screenManager)
        {
            _testManager = testManager;
            _repository = repository;
            _service = service;

            var canStartTest = this.WhenAnyValue(x => x.SelectedDeviceType, x => x.SelectedCommPort, x => x.SelectedBaudRate,
                (device, comm, baud) => device != null && !string.IsNullOrEmpty(comm) && baud != 0);

            StartTestCommand = ReactiveCommand.CreateFromTask(StartTest, canStartTest);
            StartTestCommand.Subscribe(_ => SetLastUsedSettings());

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

        #region IDisposable Members

        public void Dispose()
        {
            StartTestCommand?.Dispose();
            Task.Run(() => _testManager.Complete());
        }

        #endregion

        #region IRoutableViewModel Members

        public string UrlPathSegment => "/VerificationTests";
        public IScreen HostScreen => ScreenManager;

        #endregion

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
        }

        private async Task StartTest()
        {
            await _testManager.StartNew(SelectedDeviceType, SelectedCommPort, SelectedBaudRate, "COM3");
            var vm = new TestDetailsViewModel(ScreenManager, _testManager);


            await ScreenManager.ChangeView(vm);
        }

        private async Task LoadStatic()
        {
            var mini = await _repository.GetAsync(Guid.Parse("05d12ea8-76d9-4ac1-9fb4-5d08a58ce04d"));

//            var testVm = await _service.CreateViewModelFromVerification(mini);
            var testVm = _service.NewTest(mini.Device);
            var vm = new TestDetailsViewModel(ScreenManager, testVm);

            await ScreenManager.ChangeView(vm);
        }
    }
}
