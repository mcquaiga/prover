using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.UI.Desktop.Dialogs
{

    public partial class DialogServiceManager : ReactiveObject, IDialogServiceManager
    {
        private readonly SerialDisposable _disposer = new SerialDisposable();
        private readonly ILogger<DialogServiceManager> _logger;
        private readonly IServiceProvider _services;
        private readonly IViewLocator _viewLocator;
        private Action _onClosed;

        public DialogServiceManager(
                IServiceProvider services,
                ILogger<DialogServiceManager> logger,
                IViewLocator viewLocator = null,
                Action<IDialogServiceManager> interactionsRegistery = null)
        //: this()
        {
            _services = services;
            _viewLocator = viewLocator ?? ViewLocator.Current;
            _logger = logger ?? NullLogger<DialogServiceManager>.Instance;

            ShowDialogView = ReactiveCommand.CreateFromTask<IViewFor, DialogSession>(async (view) =>
            {
                if (DialogSession != null && !DialogSession.IsEnded)
                {
                    DialogSession.UpdateContent(view);
                }
                else
                {
                    _ = DialogHost.Show(view,
                        openedEventHandler: (sender, args) => DialogSession = args.Session,
                        closingEventHandler: (sender, args) =>
                        {
                            DialogSession = null;
                            _disposer.Disposable = Disposable.Empty;
                        });
                }

                return DialogSession;
            }, outputScheduler: RxApp.MainThreadScheduler);

            ShowDialogViewModel = ReactiveCommand.CreateFromObservable<IDialogViewModel, IViewFor>(
                viewModel =>
                {
                    return Observable.Start(() =>
                    {
                        var view = _viewLocator.ResolveView(viewModel);
                        view.ViewModel = viewModel;
                        return view;
                    }, RxApp.MainThreadScheduler);
                }, outputScheduler: RxApp.MainThreadScheduler);


            ShowDialogViewModel.InvokeCommand(ShowDialogView);

            CloseDialog = ReactiveCommand.CreateFromObservable(() =>
            {
                DialogSession?.Close();
                //_disposer.Disposable = Disposable.Empty;
                return Observable.Return((IViewFor)null);
            }, outputScheduler: RxApp.MainThreadScheduler);

            RegisterInteractions(this);
            interactionsRegistery?.Invoke(this);
        }

        public ReactiveCommand<IDialogViewModel, IViewFor> ShowDialogViewModel { get; set; }

        private ReactiveCommand<IViewFor, DialogSession> ShowDialogView { get; }
        private ReactiveCommand<Unit, IViewFor> CloseDialog { get; }
        [Reactive] public DialogSession DialogSession { get; set; }


        public async Task Close()
        {
            await CloseDialog.Execute();
        }

        public async Task Show<TView>(TView dialogView, Action onClosed = null) where TView : IViewFor
        {
            _onClosed = onClosed;
            await ShowDialogView.Execute(dialogView);
        }

        public async Task ShowViewModel<TViewModel>(TViewModel dialogViewModel, Action onClosed = null) where TViewModel : class, IDialogViewModel
        {
            await ShowDialogViewModel.Execute(dialogViewModel);
            await dialogViewModel.CloseCommand.Merge(dialogViewModel.CancelCommand).FirstAsync();
        }

        public async Task Show<T>(Action onClosed = null)
            where T : class, IDialogViewModel
        {
            _onClosed = onClosed;
            var model = _services.GetService<T>();
            await ShowViewModel(model);
        }

        public async Task<TResult> ShowInputDialog<TResult>(string message, string title = null)
        {
            var inputDialog = new InputDialogViewModel(message, title);

            await ShowViewModel(inputDialog);

            return (TResult)(object)inputDialog.InputValue;
        }

        public async Task ShowMessage(string message, string title)
        {
            var model = new TextDialogViewModel(message, title);
            await ShowViewModel(model);
        }

        public async Task<bool> ShowQuestion(string question)
        {
            var viewModel = new DialogViewModel { Message = question };
            var view = new QuestionDialogView
            {
                ViewModel = viewModel,
                MessageText = { Text = question }
            };

            await ShowDialogView.Execute(view);
            await viewModel.CloseCommand.Merge(viewModel.CancelCommand).FirstAsync();
            return view.Answer ?? false;
        }

        //private IViewFor GenerateView(IDialogViewModel viewModel)
        //{
        //    //RxApp.MainThreadScheduler.Schedule(viewModel, (scheduler, model) =>
        //    //{
        //        var view = _viewLocator.ResolveView(viewModel);
        //        if (view == null)
        //            throw new Exception($"Couldn't find view for '{viewModel.GetType()}'.");

        //        view.ViewModel = viewModel;

        //        _disposer.Disposable = Disposable.Empty;
        //        _disposer.Disposable = Disposable.Create(() =>
        //        {
        //            (view as IDisposable)?.Dispose();
        //            _onClosed?.Invoke();
        //        });

        //        return view;
        //    //});

        //}
    }


}