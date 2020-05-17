using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.QATests.Volume.Mechanical
{
    /// <summary>
    ///     Interaction logic for EnergyTest.xaml
    /// </summary>
    public partial class EnergyVolumeTestView
    {
        public EnergyVolumeTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.PercentageControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.PercentageControl.Passed).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Units, v => v.UnitsControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.TrueValueControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.EvcValueControl.Value).DisposeWith(d);
            });
        }
    }
}