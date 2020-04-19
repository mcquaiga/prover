using System.Reactive.Disposables;
using Prover.Application.ViewModels.Volume;
using Prover.UI.Desktop.Extensions;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Verifications.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class UncorrectedVolumeView : ReactiveUserControl<UncorrectedVolumeTestViewModel>
    {
        public UncorrectedVolumeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.PercentErrorControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.PercentErrorControl.Passed).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StartValues.UncorrectedReading, v => v.StartReadingControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.EndReading, v => v.EndReadingControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedValueControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.ActualValueControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.PulseOutputTest.ExpectedValue, v => v.PulsesExpectedControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PulseOutputTest.ActualValue, v => v.UncorrectedPulseCountControl.Value).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.PulseOutputTest.Verified, v => v.PulsesVerifiedControl.Value,
                //    value => value == true ? "PASS" : "FAIL").DisposeWith(d);
                //Value="{Binding Volume.UnCorPulsesPassed, Converter={StaticResource BoolToPassFailConverter}}"
                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}
