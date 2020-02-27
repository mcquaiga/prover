using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public class MainViewModel : ReactiveObject, IScreenManager, IDisposable, INavigationItem
    {
        private readonly DialogServiceManager _dialogService;
        private readonly IServiceProvider _services;

        public MainViewModel(IServiceProvider services, DialogServiceManager dialogService,
            IEnumerable<INavigationItem> navigationItems = null)
        {
            _services = services;
            _dialogService = dialogService;
            NavigationItems = navigationItems;

            Router = new RoutingState();

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Navigate);

            GoBack = Router.NavigateBack;

            //var dialogs = _dialogService;
            //dialogs.WhenAnyValue(d => d.DialogView)
            //    .ToPropertyEx(this, model => model.DialogContent);
            //dialogs.WhenAnyValue(x => x.IsOpen)
            //    .Select(x => x)
            //    .ToPropertyEx(this, x => x.DialogViewOpen);
        }

        public IEnumerable<INavigationItem> NavigationItems { get; }

        public ReactiveCommand<Unit, Unit> NavigationCommand => ReactiveCommand.Create(Router.NavigationStack.Clear);
        public PackIconKind IconKind => PackIconKind.Home;
        public bool IsHome => true;

        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        [Reactive] public DialogServiceManager DialogManager { get; set; }

        public RoutingState Router { get; }

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

        public void Dispose()
        {
            GoNext?.Dispose();
            GoBack?.Dispose();

            Router.NavigationStack.ToList().ForEach(vm => (vm as IDisposable)?.Dispose());
        }

        //_dialogService.ShowCustom<TViewModel>(owner, viewModel);
        public bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel)
            where TViewModel : IWindow => false;

        public void ShowDialog(INotifyPropertyChanged viewModel)
        {
            throw new NotImplementedException();
        }

        public void ShowMenu()
        {
            GoNext.Execute(_services.GetService<HomeViewModel>());

            //Task.Run(() => ChangeViews<HomeViewModel>());
        }

        public void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel)
        {
            //_dialogService.ShowDialog(owner, viewModel);
        }

        private IObservable<IRoutableViewModel> Navigate(IRoutableViewModel viewModel) =>
            Router.Navigate.Execute(viewModel);
    }
}