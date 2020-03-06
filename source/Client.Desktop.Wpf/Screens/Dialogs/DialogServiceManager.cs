﻿using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Screens.Dialogs
{
    public class DialogServiceManager : ReactiveObject
    {
        private readonly SerialDisposable _disposer = new SerialDisposable();
        private readonly IServiceProvider _services;
        private readonly ILogger<DialogServiceManager> _logger;

        public DialogServiceManager(IServiceProvider services, ILogger<DialogServiceManager> logger, IViewLocator viewLocator = null)
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
                    _disposer.Disposable = Disposable.Create(() => { 
                        
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
                //.ObserveOn(RxApp.TaskpoolScheduler)
                .Where(open => !open)
                .Select(_ => Unit.Default)
                .InvokeCommand(CloseDialog);

            ShowDialog
                .Merge(ShowDialogView)
                .Merge(CloseDialog)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.DialogContent);

            this.WhenAnyValue(x => x.DialogContent)
                //.ObserveOn(RxApp.TaskpoolScheduler)
                .Select(v => v?.ViewModel as IDialogViewModel)
                .ToPropertyEx(this, x => x.DialogViewModel);
        }

        [Reactive] public ReactiveCommand<IViewFor, IViewFor> ShowDialogView { get; protected set; }

        public extern IViewFor DialogContent { [ObservableAsProperty] get; }
        public extern IDialogViewModel DialogViewModel { [ObservableAsProperty] get;  }

        [Reactive] public ReactiveCommand<IDialogViewModel, IViewFor> ShowDialog { get; protected set; }
        [Reactive] public ReactiveCommand<Unit, IViewFor> CloseDialog { get; protected set; }

        public void Show(IDialogViewModel viewModel, IViewFor view)
        {

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
            var model = new QuestionDialogViewModel(question);
            await ShowDialog.Execute(model);

            await CloseDialog.FirstAsync();

            return model.Answer;
        }

    }
}