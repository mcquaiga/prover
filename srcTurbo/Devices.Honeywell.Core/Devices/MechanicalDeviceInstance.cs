using Devices.Core.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Devices
{
    public class MechanicalDeviceInstance : HoneywellDeviceInstance
    {
        public IEnergyItems Energy { get; }

        public MechanicalDeviceInstance(HoneywellDeviceType evcType, IEnumerable<ItemValue> itemValues) : base(evcType, itemValues)
        {
            Energy = GetItemValuesByGroup<IEnergyItems>();
        }
    }
}