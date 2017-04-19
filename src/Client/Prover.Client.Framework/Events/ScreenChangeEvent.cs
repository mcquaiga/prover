using Caliburn.Micro.ReactiveUI;

namespace Prover.Client.Framework.Events
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