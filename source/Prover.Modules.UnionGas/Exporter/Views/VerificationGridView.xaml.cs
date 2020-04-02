using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    ///     Interaction logic for InstrumentView.xaml
    /// </summary>
    public partial class VerificationGridView
    {
        public VerificationGridView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.AddSignedOnUser, v => v.AddUserToTestButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.EmployeeId, v => v.AddUserToTestButton.Visibility,
                    value => string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Hidden).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ExportVerification, v => v.ExportTestButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ExportedDateTime, v => v.ExportTestButton.Visibility,
                    value => value == null ? Visibility.Visible : Visibility.Hidden).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ArchiveVerification, v => v.ArchiveTestButton).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ArchivedDateTime, v => v.ArchiveTestButton.Visibility,
                    value => value == null ? Visibility.Visible : Visibility.Hidden).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ExporterViewModel.PrintReport, v => v.PrintTestReportButton, vm => vm.Test).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ExportedDateTime, v => v.IsExportedTextBlock.Text,
                    value => value != null ? "EXPORTED" : "").DisposeWith(d); 
                this.OneWayBind(ViewModel, vm => vm.ArchivedDateTime, v => v.IsArchivedTextBlock.Text,
                    value => value != null ? "ARCHIVED" : "").DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddJobId, v => v.EditJobIdButton).DisposeWith(d);

                TestDateTimeTextBlock.Text = $"{ViewModel.Test.TestDateTime:g}";

            });
        }

    }
}