using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Client.Desktop.Wpf.Screens;
using MaterialDesignThemes.Wpf;
using Prover.Application.ViewModels;
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