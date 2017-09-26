using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common.Screens.Dialogs;

namespace Prover.GUI.Common.Events
{
    public class DialogDisplayEvent
    {
        public DialogDisplayEvent(IDialogViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public IDialogViewModel ViewModel { get; }
    }
}