namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
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