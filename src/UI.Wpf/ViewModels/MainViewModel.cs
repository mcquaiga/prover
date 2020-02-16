using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Views;
using Client.Wpf.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using NLog.LayoutRenderers.Wrappers;
using ReactiveUI;
using Splat;

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
        private readonly IDialogService _dialogService;

        public RoutingState Router { get; }
        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public MainViewModel(IServiceProvider services, IDialogService dialogService)
        {
            _services = services;
            _dialogService = dialogService;

            Router = new RoutingState();

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Navigate);

            GoBack = Router.NavigateBack;
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
           _dialogService.ShowCustom<TViewModel>(owner, viewModel);
           return false;
        }

        public void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel)
        {
            _dialogService.ShowDialog(owner, viewModel);
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

        public void Dispose()
        {
            GoNext?.Dispose();
            GoBack?.Dispose();

            Router.NavigationStack.ToList().ForEach(vm => (vm as IDisposable)?.Dispose());
        }
    }
}