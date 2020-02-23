using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Screens.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels.Dialogs
{
    public class BackgroundWorkDialogViewModel : DialogViewModel
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public BackgroundWorkDialogViewModel(Func<CancellationToken, Task> workAction = null) : base()
        {
            //TaskCommand = ReactiveCommand.CreateFromTask(() => CreateTask(workAction));

            //CancelCommand = ReactiveCommand.Create(() => _cancellationTokenSource.Cancel());

            TitleText = "Adam";

            StatusText = "Rules!";
            ProgressTotal = 100;

            this.WhenAnyValue(x => x.StatusText)
                .Subscribe(Console.WriteLine);
        }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

        [Reactive] public string TitleText { get; set; }
        [Reactive] public string StatusText { get; set; }
        [Reactive] public int Progress { get; set; }
        [Reactive] public int ProgressTotal { get; set; }

        public ReactiveCommand<Unit, IObservable<Unit>> TaskCommand { get; set; }

        #region IModalDialogViewModel Members

        public bool? DialogResult { get; } = false;

        public void RegisterObservable<T, TViewModel>(IObservable<T> observable, Action<T, TViewModel> update) where TViewModel : BackgroundWorkDialogViewModel
        {
            observable
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(o => update(o, (TViewModel) this));
        }
        #endregion

        private IObservable<Unit> CreateTask(Func<CancellationToken, Task> workAction)
        {
            _cancellationTokenSource = new CancellationTokenSource(); 
            return Observable.Start(() => { workAction.Invoke(_cancellationTokenSource.Token); }, RxApp.TaskpoolScheduler);
        }
    }
}