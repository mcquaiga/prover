using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Client.Wpf.ViewModels.Dialogs;
using Client.Wpf.Views.Dialogs;
using Devices;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using DynamicData;
using ReactiveUI;

namespace Client.Wpf.ViewModels.Verifications
{
    public class NewTestViewModel : ViewModelBase, IRoutableViewModel
    {

        private readonly ReadOnlyObservableCollection<DeviceType> _deviceTypes;

        public NewTestViewModel(IScreenManager screenManager) : base(screenManager)
        {

            StartTestCommand = ReactiveCommand.CreateFromTask(StartTest);

            Repository.Get()
                .Connect()
                .Filter(d => !d.IsHidden)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _deviceTypes)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes => _deviceTypes;

        public ReactiveCommand<Unit, Unit> StartTestCommand { get; set; }

        #region IRoutableViewModel Members

        public string UrlPathSegment => "/VerificationTests";
        public IScreen HostScreen => ScreenManager;

        #endregion

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
            var model = new BackgroundWorkDialogViewModel(Begin);

            var obs = Observable.Start(() => Begin(new CancellationToken()), RxApp.TaskpoolScheduler);

            ScreenManager.ShowDialog<BackgroundWorkDialog>(this, model);
            GenerateObservable().ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    model.Progress = (int)x;
                    model.TitleText = $"Adam #{x}";
                    model.StatusText = $"Loop #{x}";
                });
            //model.RegisterObservable<long, BackgroundWorkDialogViewModel>(GenerateObservable(), (x, vm) =>
            //    {
            //        vm.Progress = (int)x;
            //        vm.TitleText = $"Adam #{x}";
            //        vm.StatusText = $"Loop #{x}";
            //    });

            await obs.RunAsync(new CancellationToken());
            await ScreenManager.ChangeView<TestDetailsViewModel>();
        }
    }
}