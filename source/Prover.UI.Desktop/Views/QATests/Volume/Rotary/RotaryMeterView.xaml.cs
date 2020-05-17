using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.QATests.Volume.Rotary
{
    /// <summary>
    /// Interaction logic for RotaryMeterView.xaml
    /// </summary>
    public partial class RotaryMeterView
    {
        public RotaryMeterView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.RotaryPercentErrorControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.RotaryPercentErrorControl.Passed).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.RotaryEvcMeterDisplacementControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.RotaryExpectedMeterDisplacementControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Items.MeterType, v => v.RotaryMeterTypeText.Text, value => value.Description).DisposeWith(d);

                // this.OneWayBind(ViewModel, vm => vm.Verified, v => v.MeterTypeInformationIcon.Foreground).DisposeWith(d);
            });
        }
    }
}
