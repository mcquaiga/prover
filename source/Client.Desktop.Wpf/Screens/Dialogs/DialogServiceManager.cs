using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Client.Desktop.Wpf.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Screens.Dialogs
{
    public class DialogServiceManager : ReactiveObject, IDialogViewModel, IDialogServiceManager
    {
        private readonly SerialDisposable _disposer = new SerialDisposable();
        private readonly ILogger<DialogServiceManager> _logger;
        private readonly IServiceProvider _services;
 
        public DialogServiceManager(IServiceProvider services, ILogger<DialogServiceManager> logger,
            IViewLocator viewLocator = null)
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

                    _disposer.Disposable = Disposable.Empty;
                    _disposer.Disposable = Disposable.Create(() =>
                    {
                        (model as IDisposable)?.Dispose();
                        (view as IDisposable)?.Dispose();
                        //onClosed?.Invoke(view.DialogContent);
                    });

                    return Observable.Return(view);
                },
                outputScheduler: RxApp.MainThreadScheduler);

            ShowDialogView = ReactiveCommand.CreateFromObservable<IViewFor, IViewFor>(view =>
                {
                    _disposer.Disposable = Disposable.Empty;
                    _disposer.Disposable = Disposable.Create(() =>
                    {
                        (view as IDisposable)?.Dispose();
                        //onClosed?.Invoke(view.DialogContent);
                    });
                    return Observable.Return(view);
                },
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
        }

        public ReactiveCommand<IViewFor, IViewFor> ShowDialogView { get; }
        public ReactiveCommand<IDialogViewModel, IViewFor> ShowDialog { get; protected set; }
        public ReactiveCommand<Unit, IViewFor> CloseDialog { get; }
        public ReactiveCommand<Unit, bool> CloseCommand { get; set; }

        public extern IViewFor DialogContent { [ObservableAsProperty] get; }
        public extern IDialogViewModel DialogViewModel { [ObservableAsProperty] get; }
        public extern bool IsDialogOpen { [ObservableAsProperty] get; }


        public async Task Close()
        {
            await CloseDialog.Execute();
        }

        public async Task Show<TView>(TView dialogView) where TView : IViewFor
        {
            await ShowDialogView.Execute(dialogView);
        }

        public async Task Show<T>()
            where T : class, IDialogViewModel
        {
            var model = _services.GetService<T>();
            await ShowDialog.Execute(model);
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

            var view = new QuestionDialogView();
            view.ViewModel = this;
            view.MessageText.Text = question;

            await ShowDialogView.Execute(view);

            await CloseDialog.FirstAsync();

            return view.Answer ?? false;
        }
    }
}