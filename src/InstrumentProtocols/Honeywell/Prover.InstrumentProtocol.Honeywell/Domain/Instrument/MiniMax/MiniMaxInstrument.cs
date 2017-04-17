using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.InstrumentProtocol.Core.Items;
using Prover.InstrumentProtocol.Core.Models.Instrument.Items;
using Prover.InstrumentProtocol.Honeywell.CommClients;

namespace Prover.InstrumentProtocol.Honeywell.Domain.Instrument.MiniMax
{
    internal class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument(int id, int accessCode, string name, string itemFilePath,
            Dictionary<string, string> itemData)
            : base(id, accessCode, name, itemFilePath, itemData)
        {
        }

        public MiniMaxInstrument(HoneywellClient commClient, int id, int accessCode, string name, string itemFilePath)
            : base(commClient, id, accessCode, name, itemFilePath)
        {
        }

        public override IVolumeItems VolumeItems => new MiniMaxVolume(this);

        public override async Task<IVolumeItems> GetVolumeItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.VolumeItems());
            return new MiniMaxVolume(items);
        }

        public override IVolumeItems GetVolumeItems(Dictionary<string, string> itemData)
        {
            var itemValues = SetupItemValues(ItemDefinitions, itemData);
            return new MiniMaxVolume(itemValues);
        }
    }
}