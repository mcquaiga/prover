using System.Reactive.Disposables;
using System.Windows;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels.Corrections;
using Prover.Shared;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    ///     Interaction logic for CorrectionTestPointView.xaml
    /// </summary>
    public partial class PressureTestView : ReactiveUserControl<PressureFactorViewModel>
    {
        public PressureTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                if (ViewModel.Items.TransducerType == PressureTransducerType.Absolute)
                {
                    AtmosphericControl.Visibility = Visibility.Visible;
                    AbsoluteControl.Visibility = Visibility.Visible;
                }

                this.Bind(ViewModel, vm => vm.PercentError, v => v.PercentError.DisplayValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Verified, v => v.PercentError.Passed).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Gauge, v => v.GaugeControl.Value).DisposeWith(d);

                //Absolute Transducers
                //this.Bind(ViewModel, vm => vm.Gauge, v => v.GasGaugeControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AtmosphericGauge, v => v.AtmosphericControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Gauge, v => v.AbsoluteControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Items.TransducerType,
                        v => v.AtmosphericControl.Visibility,
                        value => value == PressureTransducerType.Absolute ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Items.TransducerType,
                        v => v.AbsoluteControl.Visibility,
                        value => value == PressureTransducerType.Absolute ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);


                this.Bind(ViewModel, vm => vm.Items.GasPressure, v => v.EvcReadingControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedFactorControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ActualValue, v => v.ActualFactorControl.Value).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}