using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for ExporterView.xaml
    /// </summary>
    public partial class ExporterView
    {
        public ExporterView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.TestsByJobNumberViewModel,
                //    view => view.TestsByJobNumberContentControl.ViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ToolbarViewModel, v => v.VerificationsGrid.ToolbarViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SelectedItems.Count, v => v.SelectedCountTextBlock.Text)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Data.Count, v => v.TestCountTextBlock.Text,
                    value => value == 1 ? $"{value} test" : $"{value} tests").DisposeWith(d);
                
                
                this.OneWayBind(ViewModel, vm => vm.Data, v => v.VerificationsGrid.DataContext).DisposeWith(d);
                
             
                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.JobIdsList, v => v.JobIdsComboBox.ItemsSource).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.FilterIncludeExported, v => v.IncludeExportedCheckBox).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.FilterIncludeArchived, v => v.IncludeArchivedCheckBox).DisposeWith(d);

                this.WhenAnyValue(x => x.ViewModel.FilterIncludeExported)
                    .SelectMany(x => x.Execute(false))
                    .Subscribe();

                this.WhenAnyValue(x => x.ViewModel.FilterIncludeArchived)
                    .SelectMany(x => x.Execute(false))
                    .Subscribe();
                
            });
        }
    }
}
