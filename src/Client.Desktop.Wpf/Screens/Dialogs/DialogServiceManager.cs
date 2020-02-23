using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using Client.Wpf.ViewModels.Devices;
using Client.Wpf.Views.Devices;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.Screens.Dialogs
{
    public class DialogServiceManager : ReactiveObject
    {
        private readonly IServiceProvider _services;

        public DialogServiceManager(IServiceProvider services)
        {
            _services = services;

            ShowDialog = ReactiveCommand.CreateFromObservable<IDialogViewModel, Unit>(
                vm =>
                {
                    if (vm == null)
                    {
                        throw new Exception("ShowDialog must be call on an IDialogViewModel.");
                    }

                    var locator = ReactiveUI.ViewLocator.Current;
                    var view = locator.ResolveView(vm);

                    if (view == null)
                    {
                        throw new Exception($"Couldn't find view for '{vm}'.");
                    }

                    view.ViewModel = vm;
                    DialogView = view;

                    vm.IsOpen = true;

                    vm.WhenAnyValue(vm => vm.IsOpen)
                        .Subscribe(x => IsOpen = x);

                    vm.CloseCommand
                        .SelectMany(c => CloseDialog.Execute())
                        .Subscribe();

                    IsOpen = true;

                    return Observable.Return(Unit.Default);
                },
                outputScheduler: RxApp.MainThreadScheduler);

            CloseDialog = ReactiveCommand.CreateFromObservable(() =>
                {
                    IsOpen = false;
                    //(DialogView.ViewModel as IDialogViewModel).CloseCommand.Execute();
                    return Observable.Return(true);
                },
                outputScheduler: RxApp.MainThreadScheduler);
        }

        [Reactive] public bool IsOpen { get; set; }
        
        [Reactive] public IViewFor DialogView { get; set; }

        public ReactiveCommand<IDialogViewModel, Unit> ShowDialog { get; protected set; }

        public ReactiveCommand<Unit, bool> CloseDialog { get; protected set; }

        public void Show<T>(INotifyPropertyChanged ownerViewModel, DialogViewModel viewModel)
            where T : ReactiveDialog<DialogViewModel>
        {
            //if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
            //if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            
            //DialogViewModel = viewModel;

            //DialogView = _services.GetService<T>();
            //DialogView.ViewModel = DialogViewModel;
        }

        public void Show(SessionDialogView view, SessionDialogViewModel viewModel)
        {

        }
    }
}