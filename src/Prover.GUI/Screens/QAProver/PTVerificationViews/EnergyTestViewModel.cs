using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.DriveTypes;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
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
