using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests
{
    public interface ITestActionsManager
    {
        Task RunVerificationCompleteActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVolumeTestCompleteActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVerificationInitActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVolumeTestInitActions(EvcCommunicationClient commClient, Instrument instrument);
    }
}