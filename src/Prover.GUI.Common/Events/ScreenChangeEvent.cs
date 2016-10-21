using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.Common.Events
{
    public class ScreenChangeEvent
    {
        public ReactiveScreen ViewModel { get; }

        public ScreenChangeEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }
    }
}