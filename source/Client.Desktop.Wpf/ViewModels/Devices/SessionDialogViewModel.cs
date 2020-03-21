using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Communications.Status;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Devices
{
    public class SessionStatusDialogViewModel : ReactiveObject
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public SessionStatusDialogViewModel(IObservable<StatusMessage> statusStream, CancellationTokenSource cts)
        {
            RegisterStatusStream(statusStream);
        }

        public void RegisterStatusStream(IObservable<StatusMessage> statusStream)
        {
            statusStream
                .Where(x => x.LogLevel >= LogLevel.Information)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(msg =>
                {
                    TitleText = msg.TitleMessage;
                    StatusText = msg.ToString();
                }).DisposeWith(_cleanup);

            statusStream
                .OfType<ItemReadStatusMessage>()
                .Where(x => x.LogLevel >= LogLevel.Debug)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(msg =>
                {
                    if (ProgressTotal == null)
                        ProgressTotal = msg.TotalCount;

                    Progress = msg.ReadCount;
                }).DisposeWith(_cleanup);
        }

        [Reactive] public string TitleText { get; set; }
        [Reactive] public string StatusText { get; set; }
        [Reactive] public int? Progress { get; set; }
        [Reactive] public int? ProgressTotal { get; set; }


    }
}