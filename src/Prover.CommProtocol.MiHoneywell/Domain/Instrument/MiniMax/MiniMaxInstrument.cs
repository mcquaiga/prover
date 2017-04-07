using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Domain.Instrument.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniMax
{
    internal class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument(int id, int accessCode, string name, string itemFilePath)
            : base(id, accessCode, name, itemFilePath)
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
    }

    internal class MiniMaxReadOnlyInstrument : HoneywellReadOnlyInstrument
    {
        public MiniMaxReadOnlyInstrument(int id, int accessCode, string name, string itemFilePath)
            : base(id, accessCode, name, itemFilePath)
        {
        }

        public override IVolumeItems VolumeItems => new MiniMaxVolume(this);
    }
}