using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.Views.Devices;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Screens.Dialogs
{
    public class DialogServiceManager : ReactiveObject
    {
        private readonly IServiceProvider _services;
        private readonly IScreenManager _screenManager;
        private readonly SerialDisposable _disposer = new SerialDisposable();
        public DialogServiceManager(IServiceProvider services, IScreenManager screenManager, IViewLocator viewLocator = null)
        {
            _services = services;
            _screenManager = screenManager;
            _screenManager.DialogManager = this;

            var locator = viewLocator ?? ViewLocator.Current;

            ShowDialog = ReactiveCommand.CreateFromObservable<IDialogViewModel, IViewFor>(
                model =>
                {
                    if (model == null) 
                        throw new Exception("ShowDialog must be called on an IDialogViewModel.");

                    var view = locator.ResolveView(model);

                    if (view == null) 
                        throw new Exception($"Couldn't find view for '{model}'.");

                    view.ViewModel = model;
                    CreateClosingCallback(model);

                    return Observable.Return(view);
                },
                outputScheduler: RxApp.MainThreadScheduler);

            CloseDialog = ReactiveCommand.CreateFromObservable(() =>
                {
                    (DialogContent.ViewModel as IDialogViewModel)?.CloseCommand.Execute();
                    return Observable.Return(true);
                },
                outputScheduler: RxApp.MainThreadScheduler);
        }

        private void CreateClosingCallback(IDialogViewModel model)
        {
            model.CloseCommand
                .SelectMany(c => CloseDialog.Execute())
                .Subscribe();

            _disposer.Disposable = Disposable.Empty;

            var closedCallback = model.WhenAnyValue(v => v.IsDialogOpen)
                .Where(isOpen => !isOpen)
                .Subscribe(_ => _disposer.Disposable = Disposable.Empty);

            _disposer.Disposable = Disposable.Create(() =>
            {
                closedCallback.Dispose();
                model.CloseCommand.Execute();
                //onClosed?.Invoke(view.DialogContent);
            });
        }

        public extern IViewFor DialogContent { [ObservableAsProperty] get; }
        //public extern bool IsDialogOpen { [ObservableAsProperty] get; }

        public ReactiveCommand<IDialogViewModel, IViewFor> ShowDialog { get; protected set; }

        public ReactiveCommand<Unit, bool> CloseDialog { get; protected set; }

        public void Show<T>(INotifyPropertyChanged ownerViewModel, DialogViewModel viewModel)
           where T : ReactiveDialog
        {
            //if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
            //if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            
            //DialogViewModel = viewModel;

            //DialogView = _services.GetService<T>();
            //DialogView.ViewModel = DialogViewModel;
        }
    }
}