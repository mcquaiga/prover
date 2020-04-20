using System.Reactive.Disposables;
using Prover.Application.ViewModels.Volume.Rotary;
using Prover.UI.Desktop.Extensions;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Verifications.Volume.Rotary
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
                    this.OneWayBind(ViewModel, vm => vm.RotaryMeterTest, v => v.RotaryMeterTestContent.ViewModel).DisposeWith(d);


                    this.Bind(ViewModel, vm => vm.Uncorrected.AppliedInput, v => v.AppliedInputControl.Value).DisposeWith(d);
                    //this.Bind(ViewModel, vm => vm.Corrected.PulseOutputTest.ActualValue, v => v.CorrectedPulseCountControl.Value).DisposeWith(d);
                    //this.Bind(ViewModel, vm => vm.Uncorrected.PulseOutputTest.ActualValue, v => v.UncorrectedPulseCountControl.Value).DisposeWith(d);

                    this.CleanUpDefaults().DisposeWith(d);

                    Disposable.Create(() =>
                    {
                        UncorrectedVolumeContent.DisposeWith(d);
                        CorrectedVolumeContent.DisposeWith(d);
                    }).DisposeWith(d);
                });
        }
    }
}
