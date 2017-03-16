using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.Common.Instruments
{
    public abstract class EvcInstrument : IInstrument
    {
        protected EvcInstrument(int id, int accessCode, string name, string itemFilePath)
        {
            Id = id;
            AccessCode = accessCode;
            Name = name;
            ItemFilePath = itemFilePath;
        }

        public int Id { get; protected set; }
        public int AccessCode { get; protected set; }
        public string Name { get; protected set; }
        public string ItemFilePath { get; protected set; }

        public IEnumerable<ItemValue> ItemValues { get; protected set; }
        
        /// <summary>
        ///     Contains all the item numbers and meta data for a specific instrument type
        /// </summary>
        public IEnumerable<ItemMetadata> ItemDefinitions { get; protected set; }

        public abstract ItemValue GetItemValue(string item, IEnumerable<ItemValue> itemValues);
        public abstract EvcCorrectorType CorrectorType(IEnumerable<ItemValue> itemValues);
    }
}