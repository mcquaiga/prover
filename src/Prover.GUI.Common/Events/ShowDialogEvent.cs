using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Common.Events
{
    public class ShowDialogEvent
    {
        public object ViewModel { get; private set; }
        
        public ShowDialogEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }
      

    }
}