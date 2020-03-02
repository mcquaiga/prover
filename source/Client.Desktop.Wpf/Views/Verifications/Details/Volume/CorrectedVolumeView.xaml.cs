using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class CorrectedVolumeView : ReactiveUserControl<CorrectedVolumeTestViewModel>
    {
        public CorrectedVolumeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.PercentErrorControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.PercentErrorControl.Passed).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StartValues.CorrectedReading, v => v.StartReadingControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.EndValues.CorrectedReading, v => v.EndReadingControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedValueControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.ActualValueControl.Value).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}
