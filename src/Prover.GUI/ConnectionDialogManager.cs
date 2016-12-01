using System.Threading;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.GUI.Dialogs;

namespace Prover.GUI
{
    public class ConnectionDialogManager : IHandle<ConnectionStatusEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ConnectionViewModel _viewModel;
        private readonly IWindowManager _windowManager;
        private bool _isDialogOpen;

        public ConnectionDialogManager(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            //_viewModel = new ConnectionViewModel(_container);
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
                _windowManager.ShowWindow(_viewModel);
                _isDialogOpen = true;
            }
        }
    }
}