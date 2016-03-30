using System;
using Microsoft.Practices.Unity;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Events;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
{
    internal class ScreenManager
    {
        internal static async Task Change(IUnityContainer container, ReactiveScreen viewModel)
        {
            await container.Resolve<IEventAggregator>().PublishOnUIThreadAsync(new ScreenChangeEvent(viewModel));
        }

        internal static void ShowDialog(IUnityContainer container, SettingsViewModel dialogViewModel)
        {
            container.Resolve<IWindowManager>().ShowDialog(dialogViewModel, null, dialogViewModel.WindowSettings);
        }
    }
}