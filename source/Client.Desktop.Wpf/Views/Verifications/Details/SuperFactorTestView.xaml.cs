using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    ///     Interaction logic for SuperFactorTestView.xaml
    /// </summary>
    public partial class SuperFactorTestView : ReactiveUserControl<SuperFactorViewModel>
    {
        public SuperFactorTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedFactorControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.ActualFactorControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.PercentError.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.PercentError.Passed).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}