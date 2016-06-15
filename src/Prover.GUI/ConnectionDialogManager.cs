using System.Threading;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.GUI.Common;
using Prover.GUI.Dialogs;
using Prover.GUI.Screens;

namespace Prover.GUI
{
    public class ConnectionDialogManager : IHandle<ConnectionStatusEvent>
    {
        private readonly IUnityContainer _container;
        private readonly ConnectionViewModel _viewModel;
        private bool _isDialogOpen;

        public ConnectionDialogManager(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            _viewModel = new ConnectionViewModel(_container);
        }

        public void Handle(ConnectionStatusEvent message)
        {
            OpenConnectDialog();

            if (message.ConnectionStatus == ConnectionStatusEvent.Status.Disconnected)
            {
                Thread.Sleep(1000);
                _viewModel.TryClose();
                _isDialogOpen = false;
            }
        }

        private void OpenConnectDialog()
        {
            if (!_isDialogOpen)
            {
                ScreenManager.Show(_container, _viewModel);
                _isDialogOpen = true;
            }
        }
    }
}