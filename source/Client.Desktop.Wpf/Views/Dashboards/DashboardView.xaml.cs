using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Prover.Application.Dashboard;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Dashboards
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

                this.BindCommand(ViewModel, vm => vm.LoadCaches, v => v.RefreshDataButton).DisposeWith(d);

                this.WhenAnyValue(x => x.ViewModel.ApplyDateFilter)
                    .SelectMany(x => x.Execute(ViewModel.DefaultSelectedDate))
                    .Subscribe();

                
            });
        }
    }
}
