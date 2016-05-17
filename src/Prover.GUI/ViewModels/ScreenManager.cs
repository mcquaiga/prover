﻿using System;
using Microsoft.Practices.Unity;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Events;
using System.Threading.Tasks;
using System.Linq;

namespace Prover.GUI.ViewModels
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
                container.Resolve<IWindowManager>().ShowPopup(dialogViewModel, null);
        }
    }
}