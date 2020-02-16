using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Settings;
using Client.Wpf.Common;
using Client.Wpf.ViewModels.Dialogs;
using Client.Wpf.Views.Dialogs;
using Devices;
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
        private readonly CommunicationsService _commService;

        public NewTestViewModel(IScreenManager screenManager, ISettingsService settingsService, CommunicationsService commService = null) : base(
            screenManager)
        {
            _commService = commService;

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
            DeviceService.Repository.Connect()
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

        private async Task Begin(CancellationToken ct)
        {
            Console.WriteLine("Hey!");

            await Task.Run(() =>
            {
                do
                {
                } while (!ct.IsCancellationRequested);
            }, ct);
        }

        private IObservable<long> GenerateObservable()
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(500));
        }

        private async Task StartTest()
        {
            //var model = new BackgroundWorkDialogViewModel(Begin);

            //var obs = Observable.Start(() => Begin(new CancellationToken()), RxApp.TaskpoolScheduler);

            //ScreenManager.ShowDialog<BackgroundWorkDialog>(this, model);
            //GenerateObservable().ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x =>
            //    {
            //        model.Progress = (int) x;
            //        model.TitleText = $"Adam #{x}";
            //        model.StatusText = $"Loop #{x}";
            //    });
            ////model.RegisterObservable<long, BackgroundWorkDialogViewModel>(GenerateObservable(), (x, vm) =>
            ////    {
            ////        vm.Progress = (int)x;
            ////        vm.TitleText = $"Adam #{x}";
            ////        vm.StatusText = $"Loop #{x}";
            ////    });

            //await obs.RunAsync(new CancellationToken());
            //await ScreenManager.ChangeView<TestDetailsViewModel>();
            await ApplicationSettings.Instance.SaveSettings();

        }

        private async Task CreateTest()
        {
        }

        public void Dispose()
        {
            StartTestCommand?.Dispose();
        }
    }
}