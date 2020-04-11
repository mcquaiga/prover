using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Dashboards
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView
    {
        public DashboardView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.DashboardItems, v => v.DashboardItemsControl.ItemsSource).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.LoadCaches, v => v.RefreshDataButton).DisposeWith(d);
                //this.WhenAnyValue(x => x.ViewModel.LoadCaches)
                //    .SelectMany(x => x.Execute())
                //    .Subscribe();
            });
        }
    }
}
