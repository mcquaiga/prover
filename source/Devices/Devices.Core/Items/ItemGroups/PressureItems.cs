using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Prover.Shared;

namespace Devices.Core.Items.ItemGroups
{

    public class PressureItems :  ItemGroup, ICorrectionFactor, IItemGroup
    {
        #region Public Properties

        public virtual decimal AtmosphericPressure { get; set; }
        public virtual decimal Base { get; set; }
        public virtual decimal Factor { get; set; }
        public virtual decimal GasPressure { get; set; }
        public virtual int Range { get; set; }
        public virtual PressureTransducerType TransducerType { get; set; }
        public virtual PressureUnitType UnitType { get; set; }
        public virtual decimal UnsqrFactor { get; set; }

        #endregion

        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var items = itemValues.ToList();

            var site = deviceType.GetGroup<SiteInformationItems>(items);
            
            if (site.LivePressureFactor == CorrectionFactorType.Fixed) 
                return null;

            return base.SetValues(deviceType, items);
        }
    }
}