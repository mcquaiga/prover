using Devices.Core.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Devices
{
    public class MechanicalDevice : HoneywellDevice
    {
        public IEnergyItems Energy { get; }

        public MechanicalDevice(HoneywellDeviceType evcType, IEnumerable<ItemValue> itemValues) : base(evcType, itemValues)
        {
            Energy = GetItemValuesByGroup<IEnergyItems>();
        }
    }
}