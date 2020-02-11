using System;
using System.Collections.Generic;
using Devices.Core.Items;

namespace Application.ViewModels
{
    public class VerificationTestPointViewModel
    {
        public Guid Id { get; set; }
        public int Level { get; set; }

        public IEnumerable<ItemValue> BeforeValues { get; set; }

        public IEnumerable<ItemValue> AfterValues { get; set; }

        public PressureFactorViewModel Pressure { get; set; }
        public TemperatureFactorViewModel Temperature { get; set; }

        public SuperFactorViewModel SuperFactor { get; set; }

        public VolumeViewModel Volume { get; set; }
    }
}