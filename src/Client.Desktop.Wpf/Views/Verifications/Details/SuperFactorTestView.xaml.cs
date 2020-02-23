using System.Reactive.Disposables;
using Application.ViewModels.Corrections;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details
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
            });
        }
    }
}