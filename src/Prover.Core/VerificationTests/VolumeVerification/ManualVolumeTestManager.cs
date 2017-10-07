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
        public ManualVolumeTestManager(IEventAggregator eventAggregator, EvcCommunicationClient commClient, VolumeTest volumeTest) : base(eventAggregator, commClient, volumeTest)
        {
        }

        public override async Task PreTest(CancellationToken ct)
        {
            await CommClient.Connect(ct);
            
            VolumeTest.Items =
                (ICollection<ItemValue>) await CommClient.GetItemValues(CommClient.ItemDetails.VolumeItems());
            await CommClient.Disconnect();            
        }

        public override async Task ExecutingTest(CancellationToken ct)
        {
            RunningTest = true;
            ResetPulseCounts(VolumeTest);

            await Task.Run(() =>
            {
                while (RunningTest || ct.IsCancellationRequested)
                {

                }                
            }, ct);
            TestStep.OnNext(VolumeTestSteps.ExecutingTest);
        }

        public override async Task PostTest(CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    await CommClient.Connect(ct);
                    VolumeTest.AfterTestItems = await CommClient.GetItemValues(CommClient.ItemDetails.VolumeItems());
                }
                finally
                {
                    await CommClient.Disconnect();
                }               
            }, ct);
        }

        public override async Task ExecuteSyncTest(CancellationToken ct)
        {
            await Task.Run(() =>
            {
            }, ct);
        }       
    }
}

