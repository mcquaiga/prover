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

        protected override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            await commClient.Connect(ct);
            
            volumeTest.Items =
                (ICollection<ItemValue>) await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
            await commClient.Disconnect();           
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            ResetPulseCounts(volumeTest);

            await Task.Run(() =>
            {
                while (ct.IsCancellationRequested == false)
                {

                }
            }, ct);
        }

        protected override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    await commClient.Connect(ct);
                    volumeTest.AfterTestItems = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
                }
                finally
                {
                    await commClient.Disconnect();
                }               
            }, ct);
        }

        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            await Task.Run(() =>
            {
            });
        }       
    }
}

