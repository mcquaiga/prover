using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Common.Events
{
    public class ShowDialogEvent
    {
        public ShowDialogEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }

        public object ViewModel { get; private set; }
    }
}