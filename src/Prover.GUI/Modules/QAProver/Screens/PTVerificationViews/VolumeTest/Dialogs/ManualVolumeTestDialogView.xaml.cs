using System;
using System.Reactive.Linq;
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
                d(ViewModel = (ManualVolumeTestDialogViewModel) DataContext);

                d(this.WhenAnyValue(x => x.ViewModel.TaskCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ManualVolumeTestDialogViewModel) value;
        }

        public ManualVolumeTestDialogViewModel ViewModel { get; set; }
    }
}