using System.Threading.Tasks;
using Prover.CommProtocol.Common;

namespace Prover.Core.VerificationTests
{
    public interface IPreTestCommand
    {
        Task Execute(EvcCommunicationClient commClient);
    }

    public interface IPostTestCommand
    {
        Task Execute(EvcCommunicationClient commClient);
    }
}