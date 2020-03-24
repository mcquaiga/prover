using System;
using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;

namespace Prover.Core.Modules.Clients.VerificationTestActions
{
    public interface IHandleInvalidItemVerification
    {
        bool ShouldInvalidItemsBeChanged(Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> invalidItems);
    }
}