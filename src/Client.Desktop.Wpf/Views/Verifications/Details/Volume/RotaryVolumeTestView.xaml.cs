using System.Reactive.Disposables;
using Application.ViewModels;
using Application.ViewModels.Volume.Rotary;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details.Volume
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
                });
        }
    }
}
