using System;
using System.Reactive;
using System.Threading;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace Prover.GUI.Common.Screens.Dialogs
{
    public interface IDialogViewModel
    {
        bool ShowDialog { get; }
        ReactiveCommand<Unit, Unit> TaskCommand { get; }
    }

    public abstract class DialogViewModel : ReactiveScreen, IDialogViewModel, IDisposable
    {
        private bool _showDialog;
        public bool ShowDialog
        {
            get => _showDialog;
            set => this.RaiseAndSetIfChanged(ref _showDialog, value);
        }
        public ReactiveCommand<Unit, Unit> TaskCommand { get; protected set; }
        public abstract void Dispose();
        public CancellationTokenSource CancellationTokenSource { get; protected set; }
        public override void TryClose(bool? dialogResult = null)
        {
            ShowDialog = false;
            base.TryClose(dialogResult);
        }
    }
}