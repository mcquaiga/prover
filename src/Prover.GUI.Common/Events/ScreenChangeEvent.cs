using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Common.Events
{
    public class ScreenChangeEvent
    {
        public ReactiveScreen ViewModel;

        public ScreenChangeEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }
    }
}