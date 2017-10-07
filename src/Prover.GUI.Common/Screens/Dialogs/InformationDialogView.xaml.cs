using ReactiveUI;

namespace Prover.GUI.Common.Screens.Dialogs
{
    /// <summary>
    /// Interaction logic for ManualVolumeTestDialogView.xaml
    /// </summary>
    public partial class InformationDialogView : IViewFor<InformationDialogViewModel>
    {
        public InformationDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel = (InformationDialogViewModel)DataContext);               
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (InformationDialogViewModel)value;
        }

        public InformationDialogViewModel ViewModel { get; set; }
    }
}
