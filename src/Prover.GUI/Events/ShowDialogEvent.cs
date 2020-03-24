using Prover.GUI.Screens.Dialogs;

namespace Prover.GUI.Events
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