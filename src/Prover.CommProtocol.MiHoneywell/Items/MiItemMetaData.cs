using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public class MiItemMetadata : ItemMetadata
    {
        public override int Number { get; set; }
        public override string Code { get; set; }
        public override string ShortDescription { get; set; }
        public override string LongDescription { get; set; }

        public override bool? IsAlarm { get; set; }
        public override bool? IsPressure { get; set; }
        public override bool? IsPressureTest { get; set; }
        public override bool? IsTemperature { get; set; }
        public override bool? IsTemperatureTest { get; set; }
        public override bool? IsVolume { get; set; }
        public override bool? IsVolumeTest { get; set; }
        public override bool? IsSuperFactor { get; set; }

    }
}