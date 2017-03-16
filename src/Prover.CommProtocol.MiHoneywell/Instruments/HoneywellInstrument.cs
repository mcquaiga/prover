using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    public class MiniAtInstrument : HoneywellInstrument
    {
        public MiniAtInstrument()
            : base(3, 33333, "Mini-AT", "MiniATItems.xml")
        {
        }      
    }

    public class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument()
            : base(4, 33333, "Mini-Max", "MiniMaxItems.xml")
        {
        }
    }

    public abstract class HoneywellInstrument : EvcInstrument
    {
        protected HoneywellClient CommClient;

        protected HoneywellInstrument(int id, int accessCode, string name, string itemFilePath)
            : base(id, accessCode, name, itemFilePath)
        {
            ItemDefinitions = ItemFileLoader.LoadItems(this);
        }

        public override ItemValue GetItemValue(string itemCode, IEnumerable<ItemValue> itemValues)
        {
            if (string.IsNullOrEmpty(itemCode)) return null;

            var itemInfo = ItemDefinitions.FirstOrDefault(i => i.Code == itemCode);
            if (itemInfo == null) return null;

            var item = itemValues.ToList().FirstOrDefault(i => i.Metadata.Code == itemCode);

            if (item != null)
                return item;

            return new ItemValue(itemInfo, string.Empty);
        }

        public override EvcCorrectorType CorrectorType(IEnumerable<ItemValue> itemValues)
        {
            var live = "Live";
            var itemValuesList = itemValues.ToList();
            var pressureFixed = GetItemValue(ItemCodes.Pressure.FixedFactor, itemValuesList).Description == live;
            var tempFixed = GetItemValue(ItemCodes.Temperature.FixedFactor, itemValuesList).Description == live;
            var superFixed = GetItemValue(ItemCodes.Super.FixedFactor, itemValuesList).Description == live;

            if (pressureFixed && tempFixed && superFixed)
                return EvcCorrectorType.PTZ;

            if (pressureFixed)
                return EvcCorrectorType.P;

            if (tempFixed)
                return EvcCorrectorType.T;

            throw new NotSupportedException("Could not determine the corrector type.");
        }
    }
}