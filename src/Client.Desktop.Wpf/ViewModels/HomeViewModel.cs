﻿using System.Collections.Generic;
using System.Linq;
using Client.Desktop.Wpf.Screens;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
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