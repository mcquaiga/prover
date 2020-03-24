using ReactiveUI;
using System.Windows.Controls;

namespace UnionGas.MASA.Dialogs.MeterDTODialog
{
    /// <summary>
    /// Interaction logic for MeterDtoListDialogView.xaml
    /// </summary>
    public partial class MeterDtoListDialogView : UserControl, IViewFor<MeterDtoListDialogViewModel>
    {
        public MeterDtoListDialogView()
        {

            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel = (MeterDtoListDialogViewModel)DataContext;
             
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MeterDtoListDialogViewModel)value; }
        }

        public MeterDtoListDialogViewModel ViewModel { get; set; }
    }
}
