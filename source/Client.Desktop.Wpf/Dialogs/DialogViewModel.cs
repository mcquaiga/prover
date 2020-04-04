using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using MaterialDesignThemes.Wpf;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace Client.Desktop.Wpf.Dialogs
{
    public class DialogViewModel : ReactiveObject, IDialogViewModel, IDisposable, IValidatableViewModel
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
        protected CancellationTokenSource CancellationTokenSource;

        public DialogViewModel(CancellationTokenSource cancellationTokenSource = null)
        {
            CancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

            //ShowCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(true)).DisposeWith(Cleanup);
            CloseCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(Unit.Default), this.IsValid())
                .DisposeWith(Cleanup);

            CancelCommand = ReactiveCommand.CreateFromObservable(() => {
                CancellationTokenSource.Cancel();
                return Observable.Return(Unit.Default);

            }).DisposeWith(Cleanup);

            CloseCommand.Merge(CancelCommand)
                .InvokeCommand(DialogHost.CloseDialogCommand)
                .DisposeWith(Cleanup);
        }

        [Reactive] public string Title { get; set; }
        [Reactive] public string Message { get; set; }

        protected DialogViewModel() : this(new CancellationTokenSource())
        {
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

        public void Dispose()
        {
            Disposing();
            Cleanup.Dispose();
        }

        protected virtual void Disposing()
        {

        }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}