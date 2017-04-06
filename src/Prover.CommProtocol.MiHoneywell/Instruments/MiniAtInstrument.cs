using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Domain.Models.Instruments.Items;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    internal class MiniAtInstrument : HoneywellInstrument
    {
        public MiniAtInstrument(HoneywellClient commClient)
            : base(commClient, 3, 33333, "Mini-AT", "MiniATItems.xml")
        {
        }
        
        public override async Task<IVolumeItems> GetVolumeItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.VolumeItems());
            return new VolumeEvcItems(items);
        }
    }
}