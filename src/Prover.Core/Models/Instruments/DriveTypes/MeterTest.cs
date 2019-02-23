using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments.DriveTypes
{
    public class MeterTest
    {
        private readonly Instrument _instrument;

        public MeterTest(Instrument instrument)
        {
            _instrument = instrument;

            if (MeterIndex == null)
                throw new KeyNotFoundException("Could not find a meter type that match the instruments value in item 432.");
        }

        public bool MeterDisplacementHasPassed => MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD);

        public decimal MeterDisplacement
        {
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        }

        public decimal? EvcMeterDisplacement => _instrument.Items.GetItem(439).NumericValue;

        public decimal MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                    return Math.Round((decimal) ((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement * 100),
                        2);
                return 0;
            }
        }

        public MeterIndexItemDescription MeterIndex
            => (MeterIndexItemDescription) _instrument.Items.GetItem(432).Metadata.ItemDescriptions
                .FirstOrDefault(x => (x as MeterIndexItemDescription).Ids.Contains(MeterTypeId));

        public string MeterTypeDescription => MeterIndex.Description;

        public string MeterType
            => !string.IsNullOrEmpty(MeterTypeDescription)
                ? MeterTypeDescription
                : _instrument.Items.GetItem(432).Description;

        public int MeterTypeId => (int) _instrument.Items.GetItem(432).NumericValue;
    }
}