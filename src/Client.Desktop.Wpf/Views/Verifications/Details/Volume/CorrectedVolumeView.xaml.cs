using Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class CorrectedVolumeView : ReactiveUserControl<CorrectedVolumeTestViewModel>
    {
        public CorrectedVolumeView()
        {
            InitializeComponent();
        }
    }
}
