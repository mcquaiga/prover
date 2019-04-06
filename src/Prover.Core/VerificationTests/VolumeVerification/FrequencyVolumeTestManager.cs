using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class FrequencyVolumeTestManager : VolumeTestManager
    {
        public FrequencyVolumeTestManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Dispose()
        {
            
        }

       public override async Task CompleteTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct, bool readTach = true)
        {
             try
            {       
                commClient.InstrumentType= Instruments.Toc;
                await commClient.Connect();
                volumeTest.AfterTestItems = await commClient.GetVolumeItems();
                if (volumeTest.VerificationTest.FrequencyTest != null)
                {
                    volumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await commClient.GetFrequencyItems();
                }
            }
            finally
            {
                await commClient.Disconnect();
            }           
        }

       public override async Task InitializeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager)
        {
            await commClient.Connect();
            await testActionsManager.RunVolumeTestInitActions(commClient, volumeTest.Instrument);
            volumeTest.Items = await commClient.GetVolumeItems();
            volumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await commClient.GetFrequencyItems();          
            await commClient.Disconnect();     
        }

        protected override async Task RunSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        protected override async Task StartRunningVolumeTest(VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }
    
    }
}
