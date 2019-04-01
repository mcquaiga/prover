using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
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
