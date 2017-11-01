using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.Dialogs;
using ReactiveUI;
using ViewLocator = ReactiveUI.ViewLocator;

namespace Prover.GUI.Common
{
    public interface IScreenManager
    {
        IConductor Conductor { get; set; }
        T ResolveViewModel<T>()
            where T : ViewModelBase;

        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        void ChangeScreen(ViewModelBase viewModel);

        void ChangeScreen<T>(string key = null)
            where T : ViewModelBase;

        bool? ShowDialog(ViewModelBase dialogViewModel);

        bool? ShowDialog<T>(string key = null)
            where T : ViewModelBase;

        Task<object> ShowModalDialog(DialogViewModel dialogViewModel);

        void ShowWindow(ViewModelBase dialogViewModel);
    }

    public class ScreenManager : IScreenManager
    {
        private ReactiveScreen _currentView;
        private readonly IWindowManager _windowManager;

        public ScreenManager(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public IConductor Conductor { get; set; }

        public T ResolveViewModel<T>()
            where T : ViewModelBase
        {
            return IoC.Get<T>();
        }

        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public void ChangeScreen(ViewModelBase viewModel)
        {
            Conductor?.ActivateItem(viewModel);           
        }

        public void ChangeScreen<T>(string key = null)
            where T : ViewModelBase
        {
            var viewModel = IoC.Get<T>(key);

            if (viewModel == null)
                throw new NullReferenceException($"Type of {typeof(T)} was not registered.");

            if (viewModel is ReactiveScreen == false)
                throw new InvalidCastException($"{viewModel} is of {viewModel.GetType()}.");

            ChangeScreen(viewModel);
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

        public async Task<object> ShowModalDialog(DialogViewModel dialogViewModel)
        {
            var view = ViewLocator.Current.ResolveView(dialogViewModel);
            view.ViewModel = dialogViewModel;
            var result = await DialogHost.Show(view);

            return result;
        }

        public void ShowWindow(ViewModelBase dialogViewModel)
        {
            if (dialogViewModel is IWindowSettings windowsSettings)
                _windowManager.ShowWindow(dialogViewModel, null, windowsSettings.WindowSettings);
            else
                _windowManager.ShowWindow(dialogViewModel);
        }
    }
}