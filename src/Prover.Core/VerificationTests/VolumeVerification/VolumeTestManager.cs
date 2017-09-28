using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
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
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected IEventAggregator EventAggreator;
        protected readonly Subject<string> Status = new Subject<string>();
        protected readonly Subject<VolumeTestSteps> TestStep = new Subject<VolumeTestSteps>();

        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;               

        protected VolumeTestManager(IEventAggregator eventAggregator)
        {           
            EventAggreator = eventAggregator;
            
            StatusMessage
                .Subscribe(x => Log.Info(x));
        }

        public IObservable<string> StatusMessage => Status.AsObservable();
        public IObservable<VolumeTestSteps> TestStepsObservable => TestStep.AsObservable();

        public bool RunningTest { get; set; }

        public virtual async Task RunTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct,
            Subject<string> testStatus = null)
        {
            try
            {
                RunningTest = true;
                Status.OnNext("Starting volume test...");

                commClient.StatusObservable
                    .Subscribe(s => Status.OnNext(s));

                await Task.Run(async () =>
                {
                    if (SettingsManager.SettingsInstance.TestSettings.RunVolumeSyncTest)
                    {
                        TestStep.OnNext(VolumeTestSteps.RunningSyncTest);
                        await ExecuteSyncTest(commClient, volumeTest, ct);
                    }

                    TestStep.OnNext(VolumeTestSteps.PreTest);
                    await PreTest(commClient, volumeTest, ct);

                    TestStep.OnNext(VolumeTestSteps.ExecutingTest);
                    await ExecutingTest(volumeTest, ct);

                    TestStep.OnNext(VolumeTestSteps.PostTest);
                    await PostTest(commClient, volumeTest, ct);

                    Status.OnNext("Finished volume test.");
                }, ct);
            }
            catch (OperationCanceledException ex)
            {
                Status.OnNext("Volume test cancelled.");
                throw;
            }
            finally
            {
                RunningTest = false;
            }
        }

        protected abstract Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct);

        protected abstract Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct);

        protected abstract Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct);

        protected abstract Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct);

        public virtual void Dispose()
        {
            TestStep?.Dispose();
            Status?.Dispose();
        }

        protected static void ResetPulseCounts(VolumeTest volumeTest)
        {
            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }
    }
}