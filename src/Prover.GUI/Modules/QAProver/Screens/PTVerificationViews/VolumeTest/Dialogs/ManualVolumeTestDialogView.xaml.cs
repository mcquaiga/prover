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
using Prover.GUI.Common.Screens.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews.VolumeTest.Dialogs
{
    /// <summary>
    /// Interaction logic for ManualVolumeTestDialogView.xaml
    /// </summary>
    public partial class ManualVolumeTestDialogView : IViewFor<ManualVolumeTestDialogViewModel>
    {
        public ManualVolumeTestDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel = (ManualVolumeTestDialogViewModel)DataContext);

                d(this.WhenAnyValue(x => x.ViewModel.TaskCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ManualVolumeTestDialogViewModel) value; }
        }

        public ManualVolumeTestDialogViewModel ViewModel { get; set; }
    }
}
