using System;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.VerificationTests
{
    //public interface IQaRunTestManager : IDisposable
    //{
    //    Instrument Instrument { get; }
    //    Task InitializeTest(InstrumentType instrumentType, CommPort commPort);
    //    Task RunTest(int level, CancellationToken ct = new CancellationToken());
    //    Task DownloadPostVolumeTest(CancellationToken ct = new CancellationToken());
    //    Task SaveAsync();
    //    Task RunVerifier();

    //    IObservable<string> TestStatus { get; }

    //    Task DownloadPreVolumeTest();
    //}

    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }
        IObservable<string> TestStatus { get; }
        VolumeTestManager VolumeTestManager { get; set; }

        Task InitializeTest(InstrumentType instrumentType, ICommPort commPort, TestSettings testSettings,
            CancellationToken ct = new CancellationToken(), Client client = null, IObserver<string> statusObserver = null);

        Task RunCorrectionTest(int level, CancellationToken ct = new CancellationToken());
        Task RunVolumeTest(CancellationToken ct);
        Task DownloadPreVolumeTest(CancellationToken ct);
        Task DownloadPostVolumeTest(CancellationToken ct);
        Task SaveAsync();
        Task RunVerifiers();
    }

}