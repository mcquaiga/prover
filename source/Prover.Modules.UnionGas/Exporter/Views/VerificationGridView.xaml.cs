using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    ///     Interaction logic for InstrumentView.xaml
    /// </summary>
    public partial class VerificationGridView
    {
        private readonly SerialDisposable _cleanup = new SerialDisposable();

        public VerificationGridView()
        {
            InitializeComponent();

            //.Disposable = Disposable.Empty;
            _cleanup.Disposable = Disposable.Create(() =>
            {
                Debug.WriteLine("Dispose was called.");
            });

            this.WhenActivated(d =>
            {
                d.Add(_cleanup);
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(x => x != null)
                    .Do(PopulateFromViewModel)
                    .Subscribe()
                    .DisposeWith(d);
                
                this.OneWayBind(ViewModel, vm => vm.EmployeeId, v => v.EmployeeIdControl.Content)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.EmployeeId, v => v.AddUserToTestButton.Visibility,
                        value => string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Hidden)
                    .DisposeWith(d);


                this.OneWayBind(ViewModel, vm => vm.ExportedDateTime, v => v.ExportTestButton.Visibility,
                        value => value == null ? Visibility.Visible : Visibility.Hidden)
                    .DisposeWith(d);


                this.OneWayBind(ViewModel, vm => vm.ArchivedDateTime, v => v.ArchiveTestButton.Visibility,
                        value => value == null ? Visibility.Visible : Visibility.Hidden)
                    .DisposeWith(d);


                this.OneWayBind(ViewModel, vm => vm.ExportedDateTime, v => v.IsExportedTextBlock.Text,
                        value => value != null ? "EXPORTED" : "")
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ArchivedDateTime, v => v.IsArchivedTextBlock.Text,
                        value => value != null ? "ARCHIVED" : "")
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddSignedOnUser, v => v.AddUserToTestButton)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddJobId, v => v.EditJobIdButton)
                    .DisposeWith(d);
                //this.BindCommand(ViewModel, vm => vm.ExporterViewModel.PrintReport, v => v.PrintTestReportButton,
                //        vm => vm.Test)
                //    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ArchiveVerification, v => v.ArchiveTestButton)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ExportVerification, v => v.ExportTestButton)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.JobId, v => v.JobIdControl.Content)
                    .DisposeWith(d);
            });
        }


        private void PopulateFromViewModel(VerificationGridViewModel viewModel)
        {
            TestDateTimeTextBlock.Text = $"{viewModel.Test.TestDateTime:g}";
            DeviceTypeTextBlock.Text = viewModel.Test.Device.DeviceType.Name;
            CompositionTypeTextBlock.Text = viewModel.CompositionType;
            DriveTypeTextBlock.Text = viewModel.Test.DriveType.InputType.ToString();
            Site2NumberControl.Content = viewModel.DeviceInfo.SiteInfo.SiteId2;
            SerialNumberControl.Content = viewModel.DeviceInfo.SiteInfo.SerialNumber;
        }
    }
}