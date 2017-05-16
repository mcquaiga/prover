using System;
using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;

namespace Prover.GUI.Modules.Clients.Screens.ItemValidation.Designer
{
    public class DesignTimeItemValidationViewModel
    {
        public DesignTimeItemValidationViewModel()
        {
            var itemMetadata = new ItemMetadata
            {
                Number = 34,
                Code = "Base Pressure",
                ShortDescription = "Base P.",
                LongDescription = "Base Pressure"
            };

            var validItem = new ItemValue(itemMetadata, "60");
            var invalidItem = new ItemValue(itemMetadata, "50.0");
            var values = new Tuple<ItemValue, ItemValue>(validItem, invalidItem);

            InvalidItems.Add(itemMetadata, values);

            itemMetadata = new ItemMetadata
            {
                Number = 87,
                Code = "P Units ",
                ShortDescription = "P. Units",
                LongDescription = "Pressure Units",
                ItemDescriptions = new List<ItemMetadata.ItemDescription>
                {
                    new ItemMetadata.ItemDescription {Id = 0, Description = "PSIG", Value = 0},
                    new ItemMetadata.ItemDescription {Id = 1, Description = "PSIA", Value = 1}
                }
            };

            validItem = new ItemValue(itemMetadata, "0");
            invalidItem = new ItemValue(itemMetadata, "1");
            values = new Tuple<ItemValue, ItemValue>(validItem, invalidItem);

            InvalidItems.Add(itemMetadata, values);
        }

        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidItems { get; } =
            new Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>>();
    }
}