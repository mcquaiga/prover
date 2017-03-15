using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.Common.Instruments
{
    public abstract class Instrument : IInstrument
    {
        protected Instrument(int id, int accessCode, string name, string itemFilePath)
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
        
        public Dictionary<string, string> ItemValues { get; protected set; }
        public IEnumerable<ItemMetadata> ItemDefinitions { get; protected set; }
        public abstract ItemValue GetItemValue(string item, Dictionary<string, string> itemValues);
        public abstract EvcCorrectorType CorrectorType(Dictionary<string, string> itemValues);
    }
}