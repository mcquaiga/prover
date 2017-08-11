using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public interface IPulseInputService
    {
    }

    public abstract class VolumeTestManagerBase : IDisposable
    {
        protected IEventAggregator EventAggreator;
        protected IDInOutBoard FirstPortAInputBoard;
        protected IDInOutBoard FirstPortBInputBoard;

        protected bool IsFirstVolumeTest = true;
        protected Logger Log;
        protected bool RequestStopTest;
        protected CancellationTokenSource TestCancellationToken;

        protected VolumeTestManagerBase(
            IEventAggregator eventAggregator)
        {
            Log = LogManager.GetCurrentClassLogger();
            EventAggreator = eventAggregator;

            //DaqService = daqService;
            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public bool RunningTest { get; set; }

        public async Task RunTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                RunningTest = true;

                await Task.Run(async () =>
                {
                    Log.Info("Volume test started!");

                    await ExecuteSyncTest(commClient, volumeTest, ct);
                    ct.ThrowIfCancellationRequested();

                    await PreTest(commClient, volumeTest, ct);

                    await ExecutingTest(volumeTest, ct);
                    ct.ThrowIfCancellationRequested();

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

        protected abstract Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct);

        protected abstract Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct);

        protected abstract Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct);

        protected abstract Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct);

        public abstract void Dispose();
    }
}