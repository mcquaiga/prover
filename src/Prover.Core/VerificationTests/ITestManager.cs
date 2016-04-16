using System.Threading.Tasks;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests
{
    public interface ITestManager
    {
        Instrument Instrument { get; }
        InstrumentCommunicator InstrumentCommunicator { get; }

        Task DownloadPressureTestItems(int level);
        Task DownloadTemperatureTestItems(int levelNumber);
        Task DownloadVerificationTestItems(int level);
        Task SaveAsync();
        Task StartLiveRead(int itemNumber);
        Task StartVolumeTest();
        Task StopLiveRead();
        void StopVolumeTest();
    }
}