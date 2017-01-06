using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;

namespace Prover.Core.VerificationTests
{
    public interface IEvcItemReset
    {
        Task PreReset(EvcCommunicationClient commClient, IEnumerable<ItemValue> items);
        Task PreReset();
        Task PostReset(EvcCommunicationClient commClient, IEnumerable<ItemValue> items);
        Task PostReset();
    }
}