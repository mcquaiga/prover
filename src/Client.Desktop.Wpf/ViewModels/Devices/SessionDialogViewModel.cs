using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Client.Desktop.Wpf.Screens.Dialogs;
using Devices.Communications.Status;
using Devices.Core.Items;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Devices
{
    public class SessionDialogViewModel : DialogViewModel, IDialogViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();

        public SessionDialogViewModel(IObservable<StatusMessage> statusStream, CancellationTokenSource cts) : base(cts)
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

        public void CloseDialog()
        {
            CloseCommand.Execute();
        }

        public override void Dispose()
        {
            CancelCommand?.Dispose();
            _cleanup.Dispose();
            base.Dispose();
        }
    }
}