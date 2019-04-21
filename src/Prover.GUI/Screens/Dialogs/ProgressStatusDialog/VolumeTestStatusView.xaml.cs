using ReactiveUI;

namespace Prover.GUI.Screens.Dialogs.ProgressStatusDialog
{
    /// <summary>
    /// Interaction logic for VolumeTestStatusView.xaml
    /// </summary>
    public partial class VolumeTestStatusView : IViewFor<ProgressStatusDialogViewModel>
    {
        public VolumeTestStatusView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                if (ViewModel == null)
                    d(ViewModel = (ProgressStatusDialogViewModel)DataContext);
                else
                    DataContext = ViewModel;
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ProgressStatusDialogViewModel)value;
        }

        public ProgressStatusDialogViewModel ViewModel { get; set; }
    }
}
