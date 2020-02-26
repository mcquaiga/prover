using System.Reactive.Disposables;
using Prover.Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details.Volume
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
                this.OneWayBind(ViewModel, vm => vm.EndValues.UncorrectedReading, v => v.EndReadingControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedValueControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.ActualValueControl.Value).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AppliedInput, v => v.AppliedInputControl.Value).DisposeWith(d);


            });
        }
    }
}
