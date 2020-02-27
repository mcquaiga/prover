using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Screens.Dialogs
{

    public interface IDialogViewModel : IReactiveObject
    {
        ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        ReactiveCommand<Unit, bool> CancelCommand { get; set; }
        bool IsDialogOpen { get; }
    }

    public abstract class DialogViewModel : ReactiveObject, IDialogViewModel
    {
        protected DialogViewModel(CancellationTokenSource cancellationTokenSource)
        {
            CancelCommand = ReactiveCommand.Create(() =>
            {
                cancellationTokenSource.Cancel();
                return false;
            });

            ShowCommand = ReactiveCommand.Create(() => true);
            CloseCommand = ReactiveCommand.Create(() => false);

            ShowCommand
                .Merge(CancelCommand)
                .Merge(CloseCommand)
                .ToPropertyEx(this, x => x.IsDialogOpen, true);

            this.WhenAnyValue(x => x.IsDialogOpen)
                .Subscribe(x => Debug.WriteLine($"Dialog result changed {x}"));
        }

        protected DialogViewModel() : this(new CancellationTokenSource()) { }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public ReactiveCommand<Unit, bool> CancelCommand { get; set; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }
    }
}