using Caliburn.Micro;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class FrequencyTestViewModel : ReactiveObject
    {
        public Core.Models.Instruments.FrequencyTest FrequencyTest { get; private set; }
        private readonly IEventAggregator _eventAggregator;


        public FrequencyTestViewModel(IEventAggregator eventAggregator, Core.Models.Instruments.FrequencyTest frequencyTest)
        {
            FrequencyTest = frequencyTest;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

        }
    }
}
