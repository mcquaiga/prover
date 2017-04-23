using System;
using System.Windows.Media;
using Caliburn.Micro;
using Prover.GUI.Common;
using Action = System.Action;
using System.Windows.Media.Imaging;

namespace Prover.Modules.Clients
{  
    public class ClientManagerApp : AppMainMenuBase
    {
        public ClientManagerApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.Modules.Clients;component/Resources/group.png"));

        public override string AppTitle => "Manage Clients";
        public override Action ClickAction => async () => await ScreenManager.ChangeScreen<ClientManagerViewModel>();
    }
}