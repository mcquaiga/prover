using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for ExportToolbarView.xaml
    /// </summary>
    public partial class ExportToolbarView
    {
        public ExportToolbarView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.AddSignedOnUser, v => v.AddUserToTestButton, vm => vm.SelectedItems)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddJobId, v => v.EditJobIdButton, vm => vm.SelectedItems)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PrintReport, v => v.PrintTestReportButton, vm => vm.SelectedItems)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ArchiveVerification, v => v.ArchiveTestButton, vm => vm.SelectedItems)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportVerification, v => v.ExportTestButton, vm => vm.SelectedItems)
                    .DisposeWith(d);
            });
        }
    }
}
