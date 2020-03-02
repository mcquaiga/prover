using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Screens.Dialogs
{
    public interface IDialogViewModel : IReactiveObject, IDisposable
    {
        ReactiveCommand<Unit, bool> ShowCommand { get; set; }

        ReactiveCommand<Unit, bool> CloseCommand { get; set; }

        //ReactiveCommand<Unit, bool> CancelCommand { get; set; }
        bool IsDialogOpen { get; }
    }

    public abstract class DialogViewModel : ReactiveObject, IDialogViewModel
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
        protected CancellationTokenSource CancellationTokenSource;

        protected DialogViewModel(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
            CancelCommand = ReactiveCommand.Create(() =>
            {
                cancellationTokenSource.Cancel();
                CloseCommand.Execute();
            });

            ShowCommand = ReactiveCommand.Create(() => true);
            CloseCommand = ReactiveCommand.Create(() => false);

            ShowCommand.Merge(CloseCommand)
                .ToPropertyEx(this, x => x.IsDialogOpen, true)
                .DisposeWith(Cleanup);

            CancelCommand.DisposeWith(Cleanup);
            CloseCommand.DisposeWith(Cleanup);
            ShowCommand.DisposeWith(Cleanup);
        }

        protected DialogViewModel() : this(new CancellationTokenSource())
        {
        }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }

        public virtual void Dispose()
        {
            Cleanup.Dispose();
        }
    }
}