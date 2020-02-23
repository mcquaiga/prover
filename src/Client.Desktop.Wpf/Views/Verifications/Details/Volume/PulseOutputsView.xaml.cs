using Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details.Volume
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
