using Caliburn.Micro.ReactiveUI;

namespace Prover.Core.Events
{
    public class TestProgressEvent
    {
        public TestProgressEvent(ReactiveScreen activeViewModel)
        {
            ActiveViewModel = activeViewModel;
        }

        public ReactiveScreen ActiveViewModel { get; set; }
    }
}