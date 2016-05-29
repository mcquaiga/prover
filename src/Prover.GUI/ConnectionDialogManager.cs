using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.GUI.ViewModels;
using Prover.GUI.ViewModels.Dialogs;

namespace Prover.GUI
{
    public class ConnectionDialogManager : IHandle<ConnectionStatusEvent>
    {
        private readonly IUnityContainer _container;
        private bool _isDialogOpen = false;
        private readonly ConnectionViewModel _viewModel;

        public ConnectionDialogManager(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            _viewModel = new ConnectionViewModel(_container);
        }

        private void OpenConnectDialog()
        {
            if (!_isDialogOpen)
            {
                ScreenManager.Show(_container, _viewModel);
                _isDialogOpen = true;
            }
        }

        public void Handle(ConnectionStatusEvent message)
        {
            OpenConnectDialog();

            if (message.ConnectionStatus == ConnectionStatusEvent.Status.Disconnected)
            {
                System.Threading.Thread.Sleep(1000);
                _viewModel.TryClose();
                _isDialogOpen = false;
            }
        }
    }
}
