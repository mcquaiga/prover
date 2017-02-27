using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;

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