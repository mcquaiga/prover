using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.GUI.Events;

namespace Prover.GUI.Screens
{
    internal class ScreenManager
    {
        internal static async Task Change(IUnityContainer container, ReactiveScreen viewModel)
        {
            await container.Resolve<IEventAggregator>().PublishOnUIThreadAsync(new ScreenChangeEvent(viewModel));
        }

        internal static void ShowDialog(IUnityContainer container, ReactiveScreen dialogViewModel)
        {
            var windowsSettings = dialogViewModel as IWindowSettings;

            if (windowsSettings != null)
                container.Resolve<IWindowManager>().ShowDialog(dialogViewModel, null, windowsSettings.WindowSettings);
            else
                container.Resolve<IWindowManager>().ShowDialog(dialogViewModel);
        }

        internal static void Show(IUnityContainer container, ReactiveScreen dialogViewModel)
        {
            var windowsSettings = dialogViewModel as IWindowSettings;

            if (windowsSettings != null)
                container.Resolve<IWindowManager>().ShowWindow(dialogViewModel, null, windowsSettings.WindowSettings);
            else
                container.Resolve<IWindowManager>().ShowWindow(dialogViewModel);
        }
    }
}