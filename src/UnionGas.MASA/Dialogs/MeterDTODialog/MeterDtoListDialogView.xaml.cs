using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnionGas.MASA.Screens.TestsByJobNumber;

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
