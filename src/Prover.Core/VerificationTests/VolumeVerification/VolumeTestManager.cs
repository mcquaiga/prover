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
        public VolumeTest VolumeTest { get; }
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected IEventAggregator EventAggreator;
        protected readonly EvcCommunicationClient CommClient;
        protected readonly Subject<string> Status = new Subject<string>();
        protected readonly Subject<VolumeTestSteps> TestStep = new Subject<VolumeTestSteps>();

        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;               

        protected VolumeTestManager(IEventAggregator eventAggregator, EvcCommunicationClient commClient, VolumeTest volumeTest)
        {
            VolumeTest = volumeTest;
            EventAggreator = eventAggregator;
            CommClient = commClient;

            StatusMessage
                .Subscribe(x => Log.Info(x));

            CommClient.StatusObservable.Subscribe(Status);
        }

        public IObservable<string> StatusMessage => Status.AsObservable();
        public IObservable<VolumeTestSteps> TestStepsObservable => TestStep.AsObservable();

        public bool RunningTest { get; set; }

        public virtual async Task RunTest(CancellationToken ct, Subject<string> testStatus = null)
        {
            try
            {
                RunningTest = true;
                Status.OnNext("Starting volume test...");               

                await Task.Run(async () =>
                {
                    if (SettingsManager.SettingsInstance.TestSettings.RunVolumeSyncTest)
                    {
                        TestStep.OnNext(VolumeTestSteps.RunningSyncTest);
                        await ExecuteSyncTest(ct);
                    }

                    TestStep.OnNext(VolumeTestSteps.PreTest);
                    await PreTest(ct);

                    TestStep.OnNext(VolumeTestSteps.ExecutingTest);
                    await ExecutingTest(ct);

                    TestStep.OnNext(VolumeTestSteps.PostTest);
                    await PostTest(ct);

                    Status.OnNext("Finished volume test.");
                }, ct);
            }
            catch (OperationCanceledException)
            {
                Status.OnNext("Volume test cancelled.");
                throw;
            }
            finally
            {
                RunningTest = false;
            }
        }

        public abstract Task ExecuteSyncTest(CancellationToken ct);
        public abstract Task PreTest(CancellationToken ct);
        public abstract Task ExecutingTest(CancellationToken ct);
        public abstract Task PostTest(CancellationToken ct);

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