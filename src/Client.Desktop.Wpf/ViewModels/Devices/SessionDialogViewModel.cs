using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Client.Wpf.Screens.Dialogs;
using Devices.Communications.Status;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels.Devices
{
    public class SessionDialogViewModel : DialogViewModel
    {
        private readonly CompositeDisposable _cleanup;

        public SessionDialogViewModel(IObservable<StatusMessage> statusStream, CancellationTokenSource cts) : base(cts)
        {
            var b = statusStream
                .Where(x => x.LogLevel >= LogLevel.Debug)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(msg =>
                {
                    TitleText = msg.TitleMessage;
                    StatusText = msg.ToString();
                });

            var a = statusStream
                .OfType<ItemReadStatusMessage>()
                .Where(x => x.LogLevel >= LogLevel.Debug)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(msg =>
                {
                    //TitleText = msg.TitleMessage;
                    //StatusText = msg.ToString();

                    Progress = msg.ReadCount;
                    ProgressTotal = msg.TotalCount;
                });

            _cleanup = new CompositeDisposable(a, b);
        }


        [Reactive] public string TitleText { get; set; }
        [Reactive] public string StatusText { get; set; }
        [Reactive] public int Progress { get; set; }
        [Reactive] public int ProgressTotal { get; set; }

        public void CloseDialog()
        {
            IsOpen = false;
        }

        public void Dispose()
        {
            CancelCommand?.Dispose();
            _cleanup.Dispose();
        }
    }
}