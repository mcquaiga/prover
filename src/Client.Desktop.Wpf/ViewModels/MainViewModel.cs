using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public partial class MainViewModel : ReactiveObject, IDisposable
    {
        //private readonly DialogServiceManager _dialogService;
        private readonly IServiceProvider _services;

        public MainViewModel(IServiceProvider services, DialogServiceManager dialogManager,
            Func<IScreenManager, IRoutableViewModel> homeViewFactoryFunc)
        {
            _services = services;

            Router = new RoutingState();

            DialogManager = dialogManager;

            HomeViewModel = homeViewFactoryFunc.Invoke(this);

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Router.Navigate.Execute);
            NavigateBack = Router.NavigateBack;
            NavigateHome = ReactiveCommand.CreateFromTask(GoHome);

            ShowTestDialog = ReactiveCommand.CreateFromTask(async ()
                =>
            {
                //await MessageInteractions.ShowMessage.Handle("This is from an interaction!");
                var result = await DialogManager.ShowQuestion("Would you like to continue?");
                Debug.WriteLine($"Answer = {result}");
                //await MessageInteractions.ShowYesNo.Handle("Would you like to continue?");
            });

            //Router.CurrentViewModel
            //    .ToPropertyEx(this, x => x.CurrentViewModel);
        }

        public IRoutableViewModel HomeViewModel { get; }

        [Reactive] public ICollection<INavigationItem> NavigationItems { get; protected set; }

        public extern IRoutableViewModel CurrentViewModel { [ObservableAsProperty] get; }

        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
        public ReactiveCommand<Unit, Unit> NavigateHome { get; }
        public ReactiveCommand<Unit, Unit> ShowTestDialog { get; protected set; }

        public void Dispose()
        {
            GoNext?.Dispose();
            Router.NavigationStack.Reverse().ForEach(vm => (vm as IDisposable)?.Dispose());
        }

        public void ShowHome()
        {
            GoNext.Execute(HomeViewModel);
        }
    }

    public partial class MainViewModel : IScreenManager
    {
        public RoutingState Router { get; }

        [Reactive] public DialogServiceManager DialogManager { get; set; }

        public async Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Router.Navigate.Execute(viewModel).Subscribe();
            return viewModel;
            //return await GoNext.Execute(viewModel).RunAsync(cts.Token);
        }

        public async Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel
        {
            var model = _services.GetService<TViewModel>();
            Router.Navigate.Execute(model).Subscribe();
            return model;
        }

        public async Task GoBack()
        {
            var current = CurrentViewModel;

            await Router.NavigateBack.Execute();

            (current as IDisposable)?.Dispose();

            // return await Router.CurrentViewModel.LastAsync();
        }

        public async Task GoHome()
        {
            Router.NavigationStack.Reverse().ForEach(v => (v as IDisposable)?.Dispose());
            await Router.NavigateAndReset.Execute(HomeViewModel);
        }
    }
}