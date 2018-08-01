using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
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
            throw new NotImplementedException();
        }

        public override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcPostTestItemReset, bool readTach = true)
        {
             try
            {                    
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

        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcTestItemReset)
        {
            await commClient.Connect();

            volumeTest.Items = await commClient.GetVolumeItems();
       
            volumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await commClient.GetFrequencyItems();
           

            await commClient.Disconnect();     
        }

        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }
    }
}
