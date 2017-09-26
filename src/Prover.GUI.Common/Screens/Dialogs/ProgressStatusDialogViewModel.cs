using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace Prover.GUI.Common.Screens.Dialogs
{

    public class ProgressStatusDialogViewModel : DialogViewModel
    {
        public ReactiveCommand CancelCommand { get; }             
        public CancellationTokenSource CancellationTokenSource { get; }        

        public ProgressStatusDialogViewModel(string headerText, Func<IObserver<string>, CancellationToken, Task> taskFunc)
        {           
            HeaderText = headerText;
            
            CancellationTokenSource = new CancellationTokenSource();            
            var statusObserver = Observer.Create<string>(s => StatusText = s);
            TaskCommand = ReactiveCommand.CreateFromTask(() => taskFunc(statusObserver, CancellationTokenSource.Token));
            
            TaskCommand.IsExecuting
                .Subscribe(x => ShowDialog = x);

            CancelCommand = ReactiveCommand.Create(() =>
                {
                    StatusText = "Cancelling...";
                    CancellationTokenSource?.Cancel();
                },
                TaskCommand.IsExecuting);

            ShowDialog = true;
        }

        #region Reactive Properties

        private string _headerText;
        public string HeaderText
        {
            get => _headerText;
            set => this.RaiseAndSetIfChanged(ref _headerText, value);
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        private long _statusProgress;
        public long StatusProgress
        {
            get => _statusProgress;
            set => this.RaiseAndSetIfChanged(ref _statusProgress, value);
        }
        #endregion

        public override void Dispose()
        {
            CancelCommand?.Dispose();
            TaskCommand?.Dispose();
            CancellationTokenSource?.Dispose();
        }
    }
}
