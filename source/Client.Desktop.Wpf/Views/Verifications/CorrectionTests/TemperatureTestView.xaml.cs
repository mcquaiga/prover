using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.CorrectionTests
{
    /// <summary>
    /// Interaction logic for TemperatureTestView.xaml
    /// </summary>
    public partial class TemperatureTestView : ReactiveUserControl<TemperatureFactorViewModel>
    {
        public TemperatureTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.PercentError, v => v.PercentError.DisplayValue).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Verified, v => v.PercentError.Passed).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Gauge, v => v.Gauge.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Items.GasTemperature, v => v.EvcReading.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ActualValue, v => v.ActualFactor.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedFactor.Value).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}
