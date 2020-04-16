using Prover.Application.ViewModels.Volume;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Verifications.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class PulseOutputsView : ReactiveUserControl<PulseOutputTestViewModel>
    {
        public PulseOutputsView()
        {
            InitializeComponent();
        }
    }
}
