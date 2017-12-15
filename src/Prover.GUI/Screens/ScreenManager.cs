using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Events;
using Prover.GUI.Screens.Dialogs;
using ViewLocator = ReactiveUI.ViewLocator;

namespace Prover.GUI.Screens
{
    public interface IScreenManager
    {      
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
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public ScreenManager(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _windowManager = windowManager;
        }      

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
            _eventAggregator.PublishOnUIThreadAsync(new ScreenChangeEvent(viewModel));
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