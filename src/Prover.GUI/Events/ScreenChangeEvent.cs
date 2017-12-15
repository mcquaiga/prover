using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Events
{
    public class ScreenChangeEvent
    {
        public ScreenChangeEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }

        public ReactiveScreen ViewModel { get; }
    }
}