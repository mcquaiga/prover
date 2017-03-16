using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.Common.Instruments
{
    public interface IInstrument
    {
        int Id { get; }
        string Name { get; }
        IEnumerable<ItemMetadata> ItemDefinitions { get; }
        ItemValue GetItemValue(string itemCode, IEnumerable<ItemValue> itemValues);
        EvcCorrectorType CorrectorType(IEnumerable<ItemValue> itemValues);
    }
}