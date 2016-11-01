using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations
{
    public interface IVerifier
    {
        Task<object> Verify(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    }

    public interface IUpdater
    {
        Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    }
}