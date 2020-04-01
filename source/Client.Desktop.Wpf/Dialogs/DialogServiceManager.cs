using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace Client.Desktop.Wpf.Dialogs
{
    public static class DialogMessageInteractions
    {
        public static void Register(IDialogServiceManager dialogManager)
        {
            MessageInteractions.ShowMessage.RegisterHandler(async i =>
            {
                await dialogManager.ShowMessage(i.Input, "Message");
                i.SetOutput(Unit.Default);
            });

            MessageInteractions.ShowYesNo.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowQuestion(i.Input);
                i.SetOutput(answer);
            });

            MessageInteractions.ShowError.RegisterHandler(async i =>
            {
                await dialogManager.ShowMessage(i.Input, "Error");
                i.SetOutput(Unit.Default);
            });

            MessageInteractions.GetInputString.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowInputDialog<string>(i.Input);
                i.SetOutput(answer);
            });

            MessageInteractions.GetInput.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowInputDialog<object>(i.Input);
                i.SetOutput(answer);
            });

            MessageInteractions.GetInputNumber.RegisterHandler(async i =>
            {
                var answer = await dialogManager.ShowInputDialog<decimal>(i.Input);
                i.SetOutput(answer);
            });
        }
    }

    public partial class DialogServiceManager : ReactiveObject, IDialogServiceManager
    {
        private readonly SerialDisposable _disposer = new SerialDisposable();
        private readonly ILogger<DialogServiceManager> _logger;
        private readonly IServiceProvider _services;
        private Action _onClosed;

        public DialogServiceManager(IServiceProvider services, ILogger<DialogServiceManager> logger,
            IViewLocator viewLocator = null, Action<IDialogServiceManager> interactionsRegistery = null)
        : this()
        {
            _services = services;
            _logger = logger ?? NullLogger<DialogServiceManager>.Instance;
            viewLocator ??= ViewLocator.Current;

            ShowDialog = ReactiveCommand.CreateFromObservable<IDialogViewModel, IViewFor>(
                model =>
                {
                    var view = viewLocator.ResolveView(model);
                    if (view == null)
                        throw new Exception($"Couldn't find view for '{model.GetType()}'.");

                    view.ViewModel = model;

                    return SetupDialogView(view);
                },
                outputScheduler: RxApp.MainThreadScheduler);

            ShowDialogView = ReactiveCommand.CreateFromObservable<IViewFor, IViewFor>(
                SetupDialogView,
                outputScheduler: RxApp.MainThreadScheduler);

            CloseDialog = ReactiveCommand.CreateFromObservable(() =>
                {
                    _disposer.Disposable = Disposable.Empty;
                    return Observable.Return((IViewFor) null);
                },
                outputScheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(x => x.DialogViewModel.IsDialogOpen)
                .Where(open => !open)
                .Select(_ => Unit.Default)
                .InvokeCommand(CloseDialog);

            ShowDialog
                .Merge(ShowDialogView)
                .Merge(CloseDialog)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.DialogContent);

            this.WhenAnyValue(x => x.DialogContent)
                .Select(v => v?.ViewModel as IDialogViewModel)
                .ToPropertyEx(this, x => x.DialogViewModel);

            DialogMessageInteractions.Register(this);
            interactionsRegistery?.Invoke(this);
        }

        
        private ReactiveCommand<IViewFor, IViewFor> ShowDialogView { get; }
        private ReactiveCommand<IDialogViewModel, IViewFor> ShowDialog { get; }
        private ReactiveCommand<Unit, IViewFor> CloseDialog { get; }

        public extern IViewFor DialogContent { [ObservableAsProperty] get; }
        private extern IDialogViewModel DialogViewModel { [ObservableAsProperty] get; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }

        

        public async Task Close()
        {
            await CloseDialog.Execute();
        }

        public async Task Show<TView>(TView dialogView, Action onClosed = null) where TView : IViewFor
        {
            _onClosed = onClosed;
            await ShowDialogView.Execute(dialogView);
        }

        public async Task Show<T>(Action onClosed = null)
            where T : class, IDialogViewModel
        {
            _onClosed = onClosed;
            var model = _services.GetService<T>();
            await ShowDialog.Execute(model);
        }

        public async Task<TResult> ShowInputDialog<TResult>(string message, string title = null)
        {
            //CloseCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(false));
            //CloseCommand
            //    .ToPropertyEx(this, x => x.IsDialogOpen, true);

            var inputDialog = new InputDialogViewModel(message, title);

            await ShowDialog.Execute(inputDialog);
            await CloseDialog.FirstAsync();

            return (TResult) (object) inputDialog.InputValue;
        }

        public async Task ShowMessage(string message, string title)
        {
            var model = new TextDialogViewModel(message, title);

            await ShowDialog.Execute(model);
        }

        public async Task<bool> ShowQuestion(string question)
        {
            CloseCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(false));
            CloseCommand
                .ToPropertyEx(this, x => x.IsDialogOpen, true);

            var view = new QuestionDialogView
            {
                ViewModel = this,
                MessageText = {Text = question}
            };

            await ShowDialogView.Execute(view);

            await CloseDialog.FirstAsync();

            return view.Answer ?? false;
        }

        private IObservable<IViewFor> SetupDialogView(IViewFor view)
        {
            _disposer.Disposable = Disposable.Empty;
            _disposer.Disposable = Disposable.Create(() =>
            {
                (view as IDisposable)?.Dispose();
                _onClosed?.Invoke();
            });
            return Observable.Return(view);
        }
    }

    public partial class DialogServiceManager : IDialogViewModel, IValidatableViewModel
    {
        private DialogServiceManager()
        {
            CloseCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(false));
            CloseCommand
                .ToPropertyEx(this, x => x.IsDialogOpen, true);
        }

        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}