using Application.ViewModels;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details
{
    /// <summary>
    /// Interaction logic for CorrectionTestPointView.xaml
    /// </summary>
    public partial class CorrectionTestPointView : ReactiveUserControl<VerificationTestPointViewModel>
    {
        public CorrectionTestPointView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.TestNumber, v => v.TestLevelBlock.Text, value => $"Level {value+1}");
                    this.OneWayBind(ViewModel, vm => vm.Pressure, v => v.PressureContent.ViewModel);
                    this.OneWayBind(ViewModel, vm => vm.Temperature, v => v.TemperatureContent.ViewModel);
                    this.OneWayBind(ViewModel, vm => vm.SuperFactor, v => v.SuperFactorContent.ViewModel);
                });
        }
    }
}
