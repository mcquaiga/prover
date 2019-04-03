using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public enum VolumeTestSteps
    {
        PreTest,
        RunningSyncTest,
        ExecutingTest,
        PostTest
    }

    public interface IPulseInputService
    {
    }

    public abstract class VolumeTestManager : IDisposable
    {
        public VolumeTest VolumeTest { get; set; }
        public ISettingsService SettingsService { get; }

        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected IEventAggregator EventAggreator;
        protected EvcCommunicationClient CommClient { get; set; }
        protected readonly Subject<string> Status = new Subject<string>();

        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;

        protected VolumeTestManager(IEventAggregator eventAggregator, ISettingsService settingsService)
        {
     
            SettingsService = settingsService;
            EventAggreator = eventAggregator;
        }

        public IObservable<string> StatusMessage => Status.AsObservable();

        public bool RunningTest { get; set; }

        //public virtual async Task RunTest(CancellationToken ct, Subject<string> testStatus = null)
        //{
        //    try
        //    {
        //        RunningTest = true;
        //        Status.OnNext("Starting volume test...");

        //        await Task.Run(async () =>
        //        {
        //            if (SettingsService.Shared.TestSettings.RunVolumeSyncTest)
        //            {
        //                await ExecuteSyncTest(ct);
        //            }

        //            await PreTest(ct);

        //            await RunTest(ct);

        //            await CompleteTest(ct);

        //            Status.OnNext("Finished volume test.");
        //        }, ct);
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        Status.OnNext("Volume test cancelled.");
        //        throw;
        //    }
        //    finally
        //    {
        //        RunningTest = false;
        //    }
        //}

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public virtual async Task RunFullVolumeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager,
            CancellationToken ct)
        {
            try
            {
                RunningTest = true;
                CommClient = commClient;
                VolumeTest = volumeTest;

                await Task.Run(async () =>
                {
                    Log.Info("Volume test started!");

                    commClient.Status.Subscribe(Status);

                    await ExecuteSyncTest(ct);
                    ct.ThrowIfCancellationRequested();

                    await PreTest(commClient, volumeTest, testActionsManager, ct);

                    await RunTest(ct);
                    ct.ThrowIfCancellationRequested();

                    await CompleteTest(testActionsManager, ct);

                    Log.Info("Volume test finished!");
                }, ct);
            }
            catch (OperationCanceledException)
            {
                Log.Info("volume test cancellation requested.");
                throw;
            }
            finally
            {
                RunningTest = false;
            }
        }

        public abstract Task ExecuteSyncTest(CancellationToken ct);
        public abstract Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct);
        public abstract Task RunTest(CancellationToken ct);
        public abstract Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct);

        public virtual void Dispose()
        {
            Status?.Dispose();
        }

        protected static void ResetPulseCounts(VolumeTest volumeTest)
        {
            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }
    }
}