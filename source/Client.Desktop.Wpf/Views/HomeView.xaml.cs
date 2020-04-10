using System.Reactive.Disposables;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views
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
                this.OneWayBind(ViewModel, vm => vm.AppMainMenus, v => v.MenuItems.ItemsSource)
                    .DisposeWith(d);
          
            });
        }

    }
}