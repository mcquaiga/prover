using System;
using System.Reactive;
using System.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.Screens.Dialogs
{

    public interface IDialogViewModel : IReactiveObject
    {
        ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        bool IsOpen { get; set; }
    }

    public abstract class DialogViewModel : ReactiveObject, IDialogViewModel
    {
        
        protected DialogViewModel(CancellationTokenSource cancellationTokenSource)
        {
            CancelCommand = ReactiveCommand.Create(cancellationTokenSource.Cancel);

            ShowCommand = ReactiveCommand.Create(() => IsOpen = true);

            CloseCommand = ReactiveCommand.Create(() => IsOpen = false);

            this.WhenAnyValue(x => x.IsOpen).Subscribe(x => Console.WriteLine($"Dialog result changed {x}"));
        }

        protected DialogViewModel() : this(new CancellationTokenSource()) { }

        public ReactiveCommand<Unit, bool> ShowCommand { get; set; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        [Reactive] public bool IsOpen { get; set; } = false;
    }
}