using System.Collections.Generic;
using System.Linq;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public class HomeViewModel : ReactiveObject, IRoutableViewModel
    {
        public HomeViewModel(IScreenManager screen, IEnumerable<IMainMenuItem> appMainMenus)
        {
            ScreenManager = screen;
            AppMainMenus = appMainMenus.OrderBy(x => x.Order).ToList();
        }

        public IScreenManager ScreenManager { get; }

        public ICollection<IMainMenuItem> AppMainMenus { get; }

        public string UrlPathSegment => "Home";

        public IScreen HostScreen => ScreenManager;
    }
}