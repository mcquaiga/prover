using System.Collections.Generic;
using System.Linq;
using Prover.Application.Dashboard;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels
{
    public class HomeViewModel : ReactiveObject, IRoutableViewModel
    {
        public HomeViewModel(IScreenManager screen, IEnumerable<IMainMenuItem> appMainMenus, DashboardViewModel dashboard = null)
        {
            ScreenManager = screen;
            AppMainMenus = appMainMenus.OrderBy(x => x.Order).ToList();
            Dashboard = dashboard;
        }

        public IScreenManager ScreenManager { get; }

        public ICollection<IMainMenuItem> AppMainMenus { get; }

        public string UrlPathSegment => "Home";

        public IScreen HostScreen => ScreenManager;

        public DashboardViewModel Dashboard { get; }
    }
}