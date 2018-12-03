namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.Core.Communication;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class MechanicalAutoVolumeTestManager : AutoVolumeTestManager
    {
        public MechanicalAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm) : base(eventAggregator, tachComm)
        {
        }      

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {
            while (await TachometerCommunicator.ReadTach() < 100 && !ct.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }  
        }
    }
}
