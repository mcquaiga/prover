using System.Reactive.Disposables;
using Application.ViewModels.Volume.Rotary;
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
                });
        }
    }
}
