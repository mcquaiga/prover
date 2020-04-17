using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.UI.Desktop.Views
{
   
    /// <summary>
    ///     Interaction logic for MainMenuView.xaml
    /// </summary>
    [SingleInstanceView]
    public partial class HomeView
    {
        public HomeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.AppMainMenus, v => v.MenuItems.ItemsSource).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Dashboard, view => view.DashboardHostControl.ViewModel).DisposeWith(d);
            });
        }

    }
}