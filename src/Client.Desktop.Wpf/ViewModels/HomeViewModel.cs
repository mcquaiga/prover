using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Client.Wpf.Screens;
using Client.Wpf.Views;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels
{
    public class HomeViewModel : ViewModelBase, IRoutableViewModel
    {
        public HomeViewModel(IScreenManager screen, IEnumerable<IMainMenuItem> appMainMenus) : base(screen)
        {
            HostScreen = screen;
            AppMainMenus = appMainMenus.OrderBy(x => x.Order).ToList();
        }

        [Reactive]public ICollection<IMainMenuItem> AppMainMenus { get; set; }

        public string UrlPathSegment => "/Home";

        public IScreen HostScreen { get; }
    }
}