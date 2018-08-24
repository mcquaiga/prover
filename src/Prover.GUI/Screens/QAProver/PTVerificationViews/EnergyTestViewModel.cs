using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.DriveTypes;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class EnergyTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        public EnergyTestViewModel(IEventAggregator eventAggregator, Energy energy)
        {
            eventAggregator.Subscribe(this);
            EnergyTest = energy;
        }

        public Energy EnergyTest { get; set; }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => EnergyTest);
        }
    }
}
