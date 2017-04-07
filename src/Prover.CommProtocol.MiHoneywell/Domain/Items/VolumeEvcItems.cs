using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class VolumeEvcItems : IVolumeItems
    {
        protected readonly IEnumerable<ItemValue> ItemValues;

        public VolumeEvcItems(IEnumerable<ItemValue> itemValues)
        {
            ItemValues = itemValues.Where(i => i.Metadata.IsVolume == true || i.Metadata.IsVolumeTest == true);
        }

        public VolumeEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public virtual decimal CorrectedMultiplier => ItemValues.GetItem(90).NumericValue;

        public virtual decimal CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);
        public virtual string CorrectedUnits => ItemValues.GetItem(90).Description;

        public virtual decimal DriveRate => ItemValues.GetItem(98).NumericValue;
        public virtual string DriveRateDescription => ItemValues.GetItem(98).Description;

        public virtual DriveTypeDescripter DriveType
            => DriveRateDescription == "Rotary" ? DriveTypeDescripter.Rotary : DriveTypeDescripter.Mechanical;

        public virtual decimal Energy => ItemValues.GetItem(140).NumericValue;
        public virtual decimal EnergyGasValue => ItemValues.GetItem(142).NumericValue;

        public virtual string EnergyUnits => ItemValues.GetItem(141).Description;
        public virtual decimal MeterDisplacement => 0.0m;
        public virtual string MeterModel => string.Empty;

        public virtual int MeterModelId => 0;
        public virtual decimal UncorrectedMultiplier => ItemValues.GetItem(92).NumericValue;

        public virtual decimal UncorrectedReading => ItemValues.GetItem(2).NumericValue;
        public virtual string UncorrectedUnits => ItemValues.GetItem(92).Description;
    }

    internal static class HighResVolumeHelpers
    {
        public static decimal GetHighResFractionalValue(decimal highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue, CultureInfo.InvariantCulture);
            var pointLocation = highResString.IndexOf(".", StringComparison.Ordinal);

            if (highResValue > 0 && pointLocation > -1)
            {
                var result = highResString.Substring(pointLocation, highResString.Length - pointLocation);

                return Convert.ToDecimal(result);
            }

            return 0;
        }

        public static decimal GetHighResolutionItemValue(int lowResValue, decimal highResValue)
        {
            var fractional = GetHighResFractionalValue(highResValue);
            return lowResValue + fractional;
        }

        public static decimal GetHighResolutionValue(this IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return 0.0m;

            var items = itemValues as ItemValue[] ?? itemValues.ToArray();
            decimal? lowResValue = items?.GetItem(lowResItemNumber)?.NumericValue ?? 0;
            decimal? highResValue = items?.GetItem(highResItemNumber)?.NumericValue ?? 0;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int) lowResValue.Value, highResValue.Value);
        }
    }
}