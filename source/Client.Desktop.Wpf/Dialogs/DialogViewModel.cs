using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace Client.Desktop.Wpf.Dialogs
{
    public class DialogViewModel : ReactiveObject, IDialogViewModel, IDisposable, IValidatableViewModel
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
        protected CancellationTokenSource CancellationTokenSource;

        public DialogViewModel(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;

            ShowCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(true)).DisposeWith(Cleanup);
            CloseCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(false), this.IsValid()).DisposeWith(Cleanup);
            CancelCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                cancellationTokenSource.Cancel();
                return Observable.Return(false);
            }).DisposeWith(Cleanup);

            ShowCommand
                .Merge(CloseCommand)
                .Merge(CancelCommand)
                .ToPropertyEx(this, x => x.IsDialogOpen, true)
                .DisposeWith(Cleanup);
        }

        protected DialogViewModel() : this(new CancellationTokenSource())
        {
        }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public ReactiveCommand<Unit, bool> CancelCommand { get; set; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }

        public void Dispose()
        {
            Disposing();
            Cleanup.Dispose();
        }

        protected virtual void Disposing()
        {

        }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}