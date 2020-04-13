using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    internal class ScreenManager : ReactiveObject, IScreenManager
    {
        private readonly SerialDisposable _toolbarRemover = new SerialDisposable();
        private readonly SourceList<IToolbarActionItem> _toolbarItems = new SourceList<IToolbarActionItem>();

        public ReadOnlyObservableCollection<IToolbarActionItem> ToolbarItems { get; }

        public IDialogServiceManager DialogManager { get; }
        [Reactive] public RoutingState Router { get; set; }
        public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager)
        {
            _services = services;
            DialogManager = dialogManager;

            Router = new RoutingState();

            Router.CurrentViewModel.Subscribe(vm => _currentViewModel = vm);

            _toolbarItems.Connect()
                         .StartWithEmpty()
                         .Bind(out var toolbarItems)
                         .DisposeMany()
                         .Subscribe();

            ToolbarItems = toolbarItems;
        }

        public async Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            if (viewModel is IHaveToolbarItems barItems)
            {
                _toolbarRemover.Disposable = Disposable.Empty;

                var disposables = barItems.ToolbarActionItems.Select(AddToolbarItem).ToList();

                _toolbarRemover.Disposable = new CompositeDisposable(disposables.ToList());
            }

            await Router.Navigate.Execute(viewModel);
            
            return viewModel;
        }

        public async Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel
        {
            var model = 
                parameters.IsNullOrEmpty() 
                    ? _services.GetService<TViewModel>() 
                    : (TViewModel)ActivatorUtilities.CreateInstance(_services, typeof(TViewModel), parameters);

            return await ChangeView(model);
        }

        public IDisposable AddToolbarItem(IToolbarActionItem item)
        {
            _toolbarItems.Add(item);

            return Disposable.Create(() =>
            {
                _toolbarItems.Remove(item);
            });
        }

        public async Task GoBack()
        {
            var current = _currentViewModel;
           
            await Router.NavigateBack.Execute();

            _toolbarRemover.Disposable = Disposable.Empty;
            (current as IDisposable)?.Dispose();
        }

        //public void SetHome(IRoutableViewModel viewModel)
        //{
        //    _homeViewModel = viewModel;
        //}

        public async Task GoHome(IRoutableViewModel home = null)
        {
            if (_homeViewModel == null)
                _homeViewModel = home;

            _toolbarRemover.Disposable = Disposable.Empty;
            Router.NavigationStack.Reverse().ForEach(v => (v as IDisposable)?.Dispose());
            await Router.NavigateAndReset.Execute(_homeViewModel);
        }

        private readonly IServiceProvider _services;
        private IRoutableViewModel _currentViewModel;
        private IRoutableViewModel _homeViewModel;
    }
}