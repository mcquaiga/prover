using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Communications.Status;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Verifications.Dialogs
{
    public class VolumeTestDialogViewModel : DialogViewModel
    {
        private readonly CompositeDisposable _cleanup;

        public VolumeTestDialogViewModel(IObservable<VolumeTestStatusMessage> statusStream, CancellationTokenSource cts) : base(cts)
        {
            //var b = statusStream
            //    .Where(x => x.LogLevel >= LogLevel.Debug)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(msg =>
            //    {
            //        TitleText = msg.TitleMessage;
            //        StatusText = msg.ToString();
            //    });

            //var a = statusStream
            //    .OfType<ItemReadStatusMessage>()
            //    .Where(x => x.LogLevel >= LogLevel.Debug)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(msg =>
            //    {
            //        //TitleText = msg.TitleMessage;
            //        //StatusText = msg.ToString();

            //        Progress = msg.ReadCount;
            //        ProgressTotal = msg.TotalCount;
            //    });

            //_cleanup = new CompositeDisposable(a, b);
        }


        [Reactive] public string TitleText { get; set; }
        [Reactive] public string StatusText { get; set; }
        [Reactive] public int Progress { get; set; }
        [Reactive] public int ProgressTotal { get; set; }

        public void CloseDialog()
        {
            IsDialogOpen = false;
        }

        public void Dispose()
        {
            CancelCommand?.Dispose();
            _cleanup.Dispose();
        }
    }
}