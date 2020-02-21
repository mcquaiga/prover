using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Screens;
using Client.Wpf.Views.Clients;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Wpf.ViewModels.Clients
{
    public class ClientManagerViewModel : ViewModelBase, IRoutableViewModel
    {
        public ClientManagerViewModel(IScreenManager screenManager) : base(screenManager)
        {
            HostScreen = screenManager;
        }


        public string UrlPathSegment { get; } = "/ClientManager";
        public IScreen HostScreen { get; }
    }

    
}
