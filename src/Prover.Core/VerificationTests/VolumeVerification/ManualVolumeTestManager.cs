using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
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
    public sealed class ManualVolumeTestManager : VolumeTestManagerBase
    {              
        public ManualVolumeTestManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {            
        }

        public override async Task RunTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct, Subject<string> testStatus = null)
        {
            try
            {
                RunningTest = true;

                await Task.Run(async () =>
                {
                    Log.Info("Volume test started!");
                  
                    testStatus?.OnNext("Downloading pre-Test values.");
                    await PreTest(commClient, volumeTest, ct);

                    testStatus?.OnNext("Waiting for input to complete.");
                    await ExecutingTest(volumeTest, ct);
                    ct.ThrowIfCancellationRequested();

                    testStatus?.OnNext("Downloading post-test values.");
                    await PostTest(commClient, volumeTest, ct);

                    Log.Info("Volume test finished!");
                }, ct);
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("volume test cancellation requested.");
                throw;
            }
            finally
            {
                RunningTest = false;
            }
        }

        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            LogManager.GetCurrentClassLogger().Debug("Volume test sync not implemented.");
        }

        protected override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            await commClient.Connect(ct);
            
            volumeTest.Items =
                (ICollection<ItemValue>) await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
            await commClient.Disconnect();

            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                
            }
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

        public override void Dispose()
        {
        }
    }
}

