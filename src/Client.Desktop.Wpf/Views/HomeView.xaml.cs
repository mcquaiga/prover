using System.Reactive.Disposables;
using System.Windows.Controls;
using Client.Wpf.ViewModels;
using ReactiveUI;

namespace Client.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for MainMenuView.xaml
    /// </summary>
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