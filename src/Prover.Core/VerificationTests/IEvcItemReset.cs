using System.Threading.Tasks;
using Prover.CommProtocol.Common;

namespace Prover.Core.VerificationTests
{
    public interface IEvcItemReset
    {
        Task PreReset(EvcCommunicationClient commClient);
        Task PostReset(EvcCommunicationClient commClient);
    }
}