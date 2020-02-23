using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Screens.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels
{
    public interface IScreenManager : IScreen
    {
        Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel);
        Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel;

        bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel)
            where TViewModel : IWindow;

        void ShowDialog(INotifyPropertyChanged viewModel);
        void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel);
    }

    public class MainViewModel : ReactiveObject, IScreenManager, IDisposable
    {
        private readonly DialogServiceManager _dialogService;
        private readonly IServiceProvider _services;

        public MainViewModel(IServiceProvider services, DialogServiceManager dialogService)
        {
            _services = services;
            _dialogService = dialogService;

            Router = new RoutingState();

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Navigate);

            GoBack = Router.NavigateBack;


            var dialogs = _dialogService;
            dialogs.WhenAnyValue(d => d.DialogView)
                .ToPropertyEx(this, model => model.DialogContent);
            dialogs.WhenAnyValue(x => x.IsOpen)
                .Select(x => x)
                .ToPropertyEx(this, x => x.DialogViewOpen);
        }

        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public DialogServiceManager DialogManager { get; }

        public extern IViewFor DialogContent { [ObservableAsProperty] get; }
        public extern bool DialogViewOpen { [ObservableAsProperty] get; }

        #region IDisposable Members

        public void Dispose()
        {
            GoNext?.Dispose();
            GoBack?.Dispose();

            Router.NavigationStack.ToList().ForEach(vm => (vm as IDisposable)?.Dispose());
        }

        #endregion

        #region IScreen Members

        public RoutingState Router { get; }

        #endregion

        #region IScreenManager Members

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

        //_dialogService.ShowCustom<TViewModel>(owner, viewModel);
        public bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel)
            where TViewModel : IWindow => false;

        public void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel)
        {
            //_dialogService.ShowDialog(owner, viewModel);
        }

        public void ShowDialog(INotifyPropertyChanged viewModel)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void ShowMenu()
        {
            GoNext.Execute(_services.GetService<HomeViewModel>());

            //Task.Run(() => ChangeViews<HomeViewModel>());
        }

        private IObservable<IRoutableViewModel> Navigate(IRoutableViewModel viewModel) =>
            Router.Navigate.Execute(viewModel);
    }
}