using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Data;
using Prover.Application.Dashboard;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Dashboards
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView
    {
        public ICollection<IDashboardItem> Grouping { get; set; }

        public DashboardView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //Grouping = CollectionViewSource.GetDefaultView(GroupDashboardItems);
                //Grouping.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
                //Grouping.SortDescriptions.Add(new SortDescription("GroupName", ListSortDirection.Descending));
                var collectionView = FindResource("DashboardListBoxItems") as CollectionViewSource;
                collectionView.Source = ViewModel.DashboardItems;

                //this.OneWayBind(ViewModel, vm => vm.DashboardItems, v => v.DashboardItemsControl.ItemsSource).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.GroupedItems, v => v.GroupDashboardItems.ItemsSource).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DateFilters, v => v.DateFiltersControl.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DefaultSelectedDate, v => v.DateFiltersControl.SelectedItem).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.LoadCaches, v => v.RefreshDataButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RefreshData, v => v.RefreshDataButton).DisposeWith(d);

                this.WhenAnyValue(x => x.ViewModel.ApplyDateFilter)
                    .SelectMany(x => x.Execute(ViewModel.DefaultSelectedDate))
                    .SubscribeOnDispatcher()
                    .DelaySubscription(TimeSpan.FromSeconds(1))
                    .Subscribe();

                //DateFiltersControl.Mou = 3;// ViewModel.DefaultSelectedDate;
                //var rb = (RadioButton) DateFiltersControl.SelectedItem;
                //rb.IsChecked = true;
                
                //DateFiltersControl.ItemsSource.Where<ICollection<string>>(x => x == ViewModel.DefaultSelectedDate);
                //this.Bind(ViewModel, vm => vm.DefaultSelectedDate, v => v.DateFiltersControl.Items)
            });
        }
    }
}
