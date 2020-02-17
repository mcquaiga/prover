using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Devices.Communications.Status;
using Microsoft.Extensions.Logging;
using MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels.Devices
{
    public class SessionDialogViewModel : ReactiveObject
    {
        private bool? dialogResult;

        public SessionDialogViewModel(IObservable<StatusMessage> statusStream,
            CancellationTokenSource cancellationTokenSource)
        {
            statusStream
                .Where(x => x.LogLevel >= LogLevel.Information)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(msg =>
                {
                    StatusText = msg.ToString();
                });

            CancelCommand = ReactiveCommand.Create(cancellationTokenSource.Cancel);

            CloseCommand = ReactiveCommand.Create(() => DialogResult = true);

            this.WhenAnyValue(x => x.DialogResult).Subscribe(x => Console.WriteLine($"Dialog result changed {x}"));
        }

        public ReactiveCommand<Unit, bool?> CloseCommand { get; set; }

        [Reactive] public string TitleText { get; set; }
        [Reactive] public string StatusText { get; set; }
        [Reactive] public int Progress { get; set; }
        [Reactive] public int ProgressTotal { get; set; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            CancelCommand?.Dispose();
        }

        #endregion

        #region IModalDialogViewModel Members

        [Reactive] public bool? DialogResult { get; set; }

        public void CloseDialog()
        {
            DialogResult = true;
        }

        #endregion
    }
}