using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using NLog.Fluent;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class ManualVolumeTestManager : VolumeTestManager
    {
        public ManualVolumeTestManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }
      
        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        public override async Task RunTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            IEvcItemReset evcTestItemReset,
            CancellationToken ct)
        {
            return;
        }

        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcTestItemReset)
        {
            await commClient.Connect();

            volumeTest.Items = await commClient.GetVolumeItems();  
          
            await commClient.Disconnect();            
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        public override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcPostTestItemReset, bool readTach = true)
        {           
            try
            {                    
                await commClient.Connect();
                volumeTest.AfterTestItems = await commClient.GetVolumeItems();              
            }
            finally
            {
                await commClient.Disconnect();
            }           
        }

        public override void Dispose()
        {
            
        }
    }
}

