using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Common.Events
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