using Caliburn.Micro.ReactiveUI;

namespace Prover.Client.Framework.Events
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