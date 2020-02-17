using Client.Wpf.Screens.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Wpf.Screens;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels
{
    public interface IScreenManager : IScreen
    {
        Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel);
        Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel;

        bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel)
            where TViewModel : IWindow;
        void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel);
        void ShowDialog(INotifyPropertyChanged viewModel);
    }

    public class MainViewModel : ReactiveObject, IScreenManager, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly DialogGuy _dialogService;

        public RoutingState Router { get; }
        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public MainViewModel(IServiceProvider services, DialogGuy dialogService)
        {
            _services = services;
            _dialogService = dialogService;

            Router = new RoutingState();

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Navigate);

            GoBack = Router.NavigateBack;


            var dialogs =_dialogService;

            dialogs.WhenAnyValue(d => d.DialogView)
                .ToPropertyEx(this, model => model.DialogContent);

            this.WhenAnyValue(x => x.DialogContent)
                .Select(x => x != null)
                .ToPropertyEx(this, x => x.DialogViewOpen);
        }

        private IObservable<IRoutableViewModel> Navigate(IRoutableViewModel viewModel)
        {
            return Router.Navigate.Execute(viewModel);
        }

        public async Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            return await GoNext.Execute(viewModel).RunAsync(cts.Token);
        }

        public async Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel
        {
            var model = _services.GetService<TViewModel>();
            return await ChangeView(model);
        }

        public bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel) where TViewModel : IWindow
        {
           //_dialogService.ShowCustom<TViewModel>(owner, viewModel);
           return false;
        }

        public void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel)
        {
            //_dialogService.ShowDialog(owner, viewModel);
        }

        public void ShowDialog(INotifyPropertyChanged viewModel)
        {
            throw new NotImplementedException();
        }

        public void ShowMenu()
        {
            GoNext.Execute(_services.GetService<HomeViewModel>());
           
            //Task.Run(() => ChangeViews<HomeViewModel>());
        }

        public extern UserControl DialogContent { [ObservableAsProperty]get; }
        public extern bool DialogViewOpen { [ObservableAsProperty]get; }

        //[Reactive]public ReactiveUserControl<DialogManager> DialogContent { get; set; }


        [Reactive] public IDialogService DialogManager { get; set; }

        public void Dispose()
        {
            GoNext?.Dispose();
            GoBack?.Dispose();

            Router.NavigationStack.ToList().ForEach(vm => (vm as IDisposable)?.Dispose());
        }
    }
}