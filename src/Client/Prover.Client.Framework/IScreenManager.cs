using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Client.Framework.Events;
using Prover.Client.Framework.Screens;
using Prover.Client.Framework.Screens.Shell;
using ReactiveUI;
using Splat;
using IScreen = ReactiveUI.IScreen;

namespace Prover.Client.Framework
{
    public interface IScreenManager
    {
        /// <summary>
        ///     Changes screen or page in the ShellView
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        void ChangeScreen(ViewModelBase viewModel);

        void ChangeScreen<T>(string key = null)
            where T : ViewModelBase;

        void GoHome();

        T ResolveViewModel<T>()
            where T : ViewModelBase;

        bool? ShowDialog(ViewModelBase dialogViewModel);

        bool? ShowDialog<T>(string key = null)
            where T : ViewModelBase;

        void ShowWindow(ViewModelBase dialogViewModel);
    }
}