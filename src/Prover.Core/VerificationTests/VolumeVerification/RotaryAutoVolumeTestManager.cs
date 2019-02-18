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

    public class RotaryAutoVolumeTestManager : AutoVolumeTestManager
    {
        public RotaryAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm) : base(eventAggregator, tachComm)
        {
        }       

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {         
           await Task.Run(() =>
           {
               while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested)
               {
               }
               OutputBoard.StopMotor();
               Log.Debug($"Test stopped at {volumeTest.UncPulseCount}");
           });           
        }
    }
}
