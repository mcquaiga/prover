using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.GUI.Events;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class EnergyTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        public EnergyTestViewModel(IEventAggregator eventAggregator, MechanicalDrive mechanicalDriveType)
        {
            eventAggregator.Subscribe(this);
            EnergyTest = mechanicalDriveType.Energy;
        }
        
        private Energy _energyTest;
        public Energy EnergyTest
        {
            get => _energyTest;
            set => this.RaiseAndSetIfChanged(ref _energyTest, value);
        }

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => EnergyTest);
        }
    }
}