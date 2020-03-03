using System.Collections.Generic;
using System.Linq;
using Client.Desktop.Wpf.Screens;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public class HomeViewModel : RoutableViewModelBase, IRoutableViewModel
    {
        public HomeViewModel(IScreenManager screen, IEnumerable<IMainMenuItem> appMainMenus) : base(screen)
        {
            HostScreen = screen;
            AppMainMenus = appMainMenus.OrderBy(x => x.Order).ToList();
        }

        [Reactive] public ICollection<IMainMenuItem> AppMainMenus { get; set; }

        public override string UrlPathSegment => "home";

        public override IScreen HostScreen { get; }
    }

    
}