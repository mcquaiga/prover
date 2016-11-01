using System;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using NLog.LayoutRenderers.Wrappers;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using ReactiveUI;
using IScreen = ReactiveUI.IScreen;

namespace Prover.GUI.Common
{
    public interface IScreenManager
    {
        Task GoHome();
        object ResolveViewModel(Type viewModelType);

        T ResolveViewModel<T>()
            where T : ViewModelBase;

        /// <summary>
        /// Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task ChangeScreen(ViewModelBase viewModel);

        Task ChangeScreen<T>(string key = null)
            where T : ViewModelBase;

        bool? ShowDialog(ViewModelBase dialogViewModel);
        void ShowWindow(ViewModelBase dialogViewModel);
    }

    public class ScreenManager : IScreenManager, IScreen
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _container;

        public RoutingState Router
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ScreenManager(IContainer container, IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _container = container;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }

        public async Task GoHome()
        {
            var main = _container.Resolve<MainMenuViewModel>();
            await ChangeScreen(main);
        }

        public object ResolveViewModel(Type viewModelType)
        {
            var viewModel = _container.Resolve(viewModelType);
            return viewModel;
        }

        public T ResolveViewModel<T>()
            where T : ViewModelBase
        {
            var viewModel = _container.Resolve<T>();
            return viewModel;
        }

        /// <summary>
        /// Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task ChangeScreen(ViewModelBase viewModel)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new ScreenChangeEvent(viewModel));
        }

        public async Task ChangeScreen<T>(string key = null)
            where T : ViewModelBase
        {
            var viewModel = _container.Resolve<T>(key);

            if (viewModel == null)
                throw new NullReferenceException($"Type of {typeof(T)} was not registered.");

            if (viewModel is ReactiveScreen == false)
                throw new InvalidCastException($"{viewModel} is of {viewModel.GetType()}.");

            await ChangeScreen(viewModel);
        }

        public bool? ShowDialog(ViewModelBase dialogViewModel)
        {
            var windowsSettings = dialogViewModel as IWindowSettings;

            if (windowsSettings != null)
                return _windowManager.ShowDialog(dialogViewModel, null, windowsSettings.WindowSettings);
            else
                return _windowManager.ShowDialog(dialogViewModel);
        }

        public void ShowWindow(ViewModelBase dialogViewModel)
        {
            var windowsSettings = dialogViewModel as IWindowSettings;

            if (windowsSettings != null)
                _windowManager.ShowWindow(dialogViewModel, null, windowsSettings.WindowSettings);
            else
                _windowManager.ShowWindow(dialogViewModel);
        }
    }
}