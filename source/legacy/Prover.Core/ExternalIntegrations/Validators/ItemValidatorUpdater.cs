using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations.Validators
{
    public interface IGetValue
    {
        string GetValue();
    }

    public interface IUpdater
    {
        Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument, CancellationToken ct);
    }

    public interface IValidator
    {
        Task<object> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    }
   
}