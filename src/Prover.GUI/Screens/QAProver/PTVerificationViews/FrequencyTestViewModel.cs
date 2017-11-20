using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
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
