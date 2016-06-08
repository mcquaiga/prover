using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata metadata, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            RawValue = value;
            Metadata = metadata;
        }

        public string RawValue { get; private set; }
        public ItemMetadata Metadata { get; private set; }

        public override string ToString()
        {
            return  $"{Environment.NewLine}================================================={Environment.NewLine}" +
                    $"{Metadata.LongDescription} - #{Metadata.Number} {Environment.NewLine}" +
                    $"   Item Value: {RawValue} {Environment.NewLine}" +
                    $"   Item Description: {Description} {Environment.NewLine}" +
                    $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        } 

        public virtual decimal NumericValue => ItemDescription?.Value ?? decimal.Parse(RawValue);

        public virtual string Description => ItemDescription?.Description ?? "[NULL]";

        private ItemMetadata.ItemDescription ItemDescription
        {
            get
            {
                if (Metadata.ItemDescriptions.Any())
                {
                    var intValue = Convert.ToInt32(RawValue);
                    return Metadata.ItemDescriptions.FirstOrDefault(x => x.Id == intValue);
                }

                return null;
            }
        } 
    }
}