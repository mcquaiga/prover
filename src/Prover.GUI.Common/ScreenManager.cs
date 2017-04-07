using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using ReactiveUI;
using Splat;
using IScreen = ReactiveUI.IScreen;

namespace Prover.GUI.Common
{
    public interface IScreenManager
    {
        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task ChangeScreen(ViewModelBase viewModel);

        Task ChangeScreen<T>(string key = null)
            where T : ViewModelBase;

        Task GoHome();

        T ResolveViewModel<T>()
            where T : ViewModelBase;

        bool? ShowDialog(ViewModelBase dialogViewModel);

        bool? ShowDialog<T>(string key = null)
            where T : ViewModelBase;

        void ShowWindow(ViewModelBase dialogViewModel);
    }

    public class ScreenManager : IScreenManager, IScreen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public ScreenManager(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }

        public RoutingState Router
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Changes screen or page in the ShellView
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
            var viewModel = IoC.Get<T>(key);

            if (viewModel == null)
                throw new NullReferenceException($"Type of {typeof(T)} was not registered.");

            if (viewModel is ReactiveScreen == false)
                throw new InvalidCastException($"{viewModel} is of {viewModel.GetType()}.");

            await ChangeScreen(viewModel);
        }

        public async Task GoHome()
        {
            var main = (MainMenuViewModel) Locator.CurrentMutable.GetService(typeof(MainMenuViewModel));
            await ChangeScreen(main);
        }

        public T ResolveViewModel<T>()
            where T : ViewModelBase
        {
            return IoC.Get<T>();
        }

        public bool? ShowDialog(ViewModelBase dialogViewModel)
        {
            var windowsSettings = dialogViewModel as IWindowSettings;

            if (windowsSettings != null)
                return _windowManager.ShowDialog(dialogViewModel, null, windowsSettings.WindowSettings);

            return _windowManager.ShowDialog(dialogViewModel);
        }

        public bool? ShowDialog<T>(string key = null)
            where T : ViewModelBase
        {
            var viewModel = IoC.Get<T>();
            return ShowDialog(viewModel);
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