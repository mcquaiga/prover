﻿using System.Reactive.Disposables;
using Client.Desktop.Wpf.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views
{
   
    /// <summary>
    ///     Interaction logic for MainMenuView.xaml
    /// </summary>
    [SingleInstanceView]
    public partial class HomeView : ReactiveUserControl<HomeViewModel>
    {
        public HomeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.AppMainMenus, v => v.MenuItems.ItemsSource)
                    .DisposeWith(d);
          
            });
        }

    }
}