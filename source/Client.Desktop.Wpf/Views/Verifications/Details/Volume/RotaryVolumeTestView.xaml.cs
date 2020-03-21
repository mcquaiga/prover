using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels.Volume.Rotary;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details.Volume
{
    /// <summary>
    /// Interaction logic for RotaryVolumeTestView.xaml
    /// </summary>
    public partial class RotaryVolumeTestView : ReactiveUserControl<RotaryVolumeViewModel>
    {
        public RotaryVolumeTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.Uncorrected, v => v.UncorrectedVolumeContent.ViewModel).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.Corrected, v => v.CorrectedVolumeContent.ViewModel).DisposeWith(d);

                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest.PercentError, v => v.RotaryPercentErrorControl.DisplayValue).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest.Verified, v => v.RotaryPercentErrorControl.Passed).DisposeWith(d);

                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest.ActualValue, v => v.RotaryEvcMeterDisplacementControl.Value).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest.ExpectedValue, v => v.RotaryExpectedMeterDisplacementControl.Value).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest.Items.MeterType, v => v.RotaryMeterTypeText.Text).DisposeWith(d);

                    this.Bind(ViewModel, vm => vm.Uncorrected.AppliedInput, v => v.AppliedInputControl.Value).DisposeWith(d);
                    this.Bind(ViewModel, vm => vm.Corrected.PulseOutputTest.ExpectedValue, v => v.CorrectedPulseCountControl.Value).DisposeWith(d);
                    this.Bind(ViewModel, vm => vm.Uncorrected.PulseOutputTest.ExpectedValue, v => v.UncorrectedPulseCountControl.Value).DisposeWith(d);

                    this.CleanUpDefaults().DisposeWith(d);

                    Disposable.Create(() =>
                    {
                        UncorrectedVolumeContent.DisposeWith(d);
                        CorrectedVolumeContent.DisposeWith(d);
                    }).DisposeWith(d);
                });
        }
    }
}
