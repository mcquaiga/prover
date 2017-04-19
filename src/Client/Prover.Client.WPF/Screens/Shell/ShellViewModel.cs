using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Client.Framework;
using Prover.Client.Framework.Screens;
using Prover.Client.Framework.Screens.Settings;
using Prover.Client.Framework.Screens.Toolbar;
using Prover.Client.Framework.Settings;
using ReactiveUI;
using Splat;
using IScreen = ReactiveUI.IScreen;

namespace Prover.Client.WPF.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveScreen>.Collection.OneActive, IScreenManager
    {
        private readonly IWindowManager _windowManager;
        private ViewModelBase _currentView;
        public string Title => "EVC Prover";
        public IEnumerable<IToolbarItem> ToolbarItems { get; set; }

        public ShellViewModel(IWindowManager windowManager, IEnumerable<IToolbarItem> toolbarItems)
        {
            _windowManager = windowManager;
            ToolbarItems = toolbarItems;
        }

        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public void ChangeScreen(ViewModelBase viewModel)
        {
            if (_currentView != viewModel)
            {
                DeactivateItem(_currentView, true);
            }

            ActivateItem(viewModel);
            _currentView = viewModel;
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

        public void GoHome()
        {
            var main = (MainMenuViewModel)Locator.CurrentMutable.GetService(typeof(MainMenuViewModel));
            ChangeScreen(main);
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