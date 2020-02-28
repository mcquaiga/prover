using System;
using System.Diagnostics;
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
        private CompositeDisposable _cleanup;

        protected DialogViewModel(CancellationTokenSource cancellationTokenSource)
        {
            CancelCommand = ReactiveCommand.Create(() =>
            {
                cancellationTokenSource.Cancel();
                CloseCommand.Execute();
            });

            ShowCommand = ReactiveCommand.Create(() => true);
            CloseCommand = ReactiveCommand.Create(() => false);

            ShowCommand.Merge(CloseCommand)
                .ToPropertyEx(this, x => x.IsDialogOpen, true);

            _cleanup = new CompositeDisposable(ShowCommand, CloseCommand, CancelCommand);
        }

        protected DialogViewModel() : this(new CancellationTokenSource()) { }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }

        public virtual void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}