using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class EnergyTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        public EnergyTestViewModel(IEventAggregator eventAggregator, MechanicalDrive mechanicalDriveType)
        {
            eventAggregator.Subscribe(this);
            EnergyTest = mechanicalDriveType.Energy;
        }

        public Energy EnergyTest { get; set; }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => EnergyTest);
        }
    }
}