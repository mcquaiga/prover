using System.Threading.Tasks;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using System.Collections.Generic;

namespace Prover.Core.VerificationTests
{
    public interface IVolumeVerification
    {
        Task StartVolumeTest();
        Task RunningVolumeTest();
        Task FinishVolumeTest();
    }

    //public interface ITestManager
    //{
    //    Instrument Instrument { get; }
    //    InstrumentCommunicator InstrumentCommunicator { get; }

    //    Task DownloadPressureTestItems(int level, bool disconnectAfter = true);
    //    Task DownloadTemperatureTestItems(int levelNumber, bool disconnectAfter = true);
    //    Task DownloadVerificationTestItems(int level);
    //    Task SaveAsync();
    //    Task StartLiveRead();
    //    Task StartLiveRead(IEnumerable<int> itemNumbers);
    //    Task PreTest();
    //    Task StopLiveRead();
    //    void StopVolumeTest();
    //}
}