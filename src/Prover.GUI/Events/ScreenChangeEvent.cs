using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Screens;

namespace Prover.GUI.Events
{
    public class ScreenChangeEvent
    {
        public ScreenChangeEvent(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
        }

        public ViewModelBase ViewModel { get; }
    }
}