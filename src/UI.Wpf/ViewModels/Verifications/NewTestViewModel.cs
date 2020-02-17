using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.Settings;
using Client.Wpf.Common;
using Client.Wpf.Communications;
using Client.Wpf.ViewModels.Dialogs;
using Client.Wpf.Views.Dialogs;
using Devices;
using Devices.Communications;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels.Verifications
{
    public class NewTestViewModel : ViewModelBase, IRoutableViewModel, IDisposable
    {
        private readonly DeviceSessionManager _sessionManager;
       

        public NewTestViewModel(IScreenManager screenManager, DeviceSessionManager sessionManager) : base(
            screenManager)
        {
            _sessionManager = sessionManager;

            StartTestCommand = ReactiveCommand.CreateFromTask(StartTest);

            SetupScreenData();
        }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; set; }

        public ReadOnlyObservableCollection<int> BaudRates { get; set; }

        public ReadOnlyObservableCollection<string> CommPorts { get; set; }

        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        #region IRoutableViewModel Members

        public string UrlPathSegment => "/VerificationTests";
        public IScreen HostScreen => ScreenManager;

        [Reactive] public string SelectedTachCommPort { get; set; }
        [Reactive] public string SelectedCommPort { get; set; }
        [Reactive] public DeviceType SelectedDeviceType { get; set; }
        [Reactive] public int SelectedBaudRate { get; set; }

        #endregion

        private void SetupScreenData()
        {
            RepositoryFactory.Instance.Connect()
                .Filter(d => !d.IsHidden)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var deviceTypes)
                .Subscribe();
            DeviceTypes = deviceTypes;

            SerialPort.GetPortNames().AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var ports)
                .Subscribe();
            CommPorts = ports;

            SerialPort.BaudRates.AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var baudRates)
                .Subscribe();
            BaudRates = baudRates;
        }

        private IObservable<long> GenerateObservable()
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(500));
        }

        private async Task StartTest()
        {
            await _sessionManager.StartSession(SelectedDeviceType, SelectedCommPort, SelectedBaudRate, this);
            await _sessionManager.DownloadItemFile();
        }


        public void Dispose()
        {
            StartTestCommand?.Dispose();
            Task.Run(() => _sessionManager.EndSession());
        }
    }
}