using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using ReactiveUI;
using Splat;

namespace Prover.GUI.Common
{
    public interface IScreenManager
    {
        Task GoHome();

        T ResolveViewModel<T>()
            where T : ViewModelBase;

        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task ChangeScreen(ViewModelBase viewModel);

        Task ChangeScreen<T>(string key = null)
            where T : ViewModelBase;

        bool? ShowDialog(ViewModelBase dialogViewModel);

        bool? ShowDialog<T>(string key = null) where T : ViewModelBase;

        bool? ShowDialog<T>(T dialogViewModel, IViewFor<T> dialogView) where T : class;

        void ShowWindow(ViewModelBase dialogViewModel);
        

        IWindowManager WindowManager { get; }
    }

    public class ScreenManager : IScreenManager
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public ScreenManager(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
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

        public IWindowManager WindowManager { get { return _windowManager; } }

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

        public bool? ShowDialog(ViewModelBase dialogViewModel)
        {

            if (dialogViewModel is IWindowSettings windowsSettings)
                return _windowManager.ShowDialog(dialogViewModel, null, windowsSettings.WindowSettings);

            return _windowManager.ShowDialog(dialogViewModel);
        }

        public bool? ShowDialog<T>(string key = null)
            where T : ViewModelBase
        {
            var viewModel = IoC.Get<T>();
            return ShowDialog(viewModel);
        }


        public bool? ShowDialog<T>(T dialogViewModel, IViewFor<T> dialogView)
            where T : class
        {
             _windowManager.ShowPopup(dialogViewModel, dialogView);
            return true;
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