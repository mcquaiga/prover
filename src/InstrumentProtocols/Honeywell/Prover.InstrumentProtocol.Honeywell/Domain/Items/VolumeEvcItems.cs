using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prover.InstrumentProtocol.Core.Items;
using Prover.InstrumentProtocol.Core.Models.Instrument.Items;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;
using Prover.Shared.Enums;

namespace Prover.InstrumentProtocol.Honeywell.Domain.Items
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

        public virtual double CorrectedMultiplier => ItemValues.GetItem(90).NumericValue;

        public virtual double CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);
        public virtual string CorrectedUnits => ItemValues.GetItem(90).Description;

        public virtual double DriveRate => ItemValues.GetItem(98).NumericValue;
        public virtual string DriveRateDescription => ItemValues.GetItem(98).Description;

        public virtual DriveTypeDescripter DriveType
            => DriveRateDescription == "Rotary" ? DriveTypeDescripter.Rotary : DriveTypeDescripter.Mechanical;

        public virtual double Energy => ItemValues.GetItem(140).NumericValue;
        public virtual double EnergyGasValue => ItemValues.GetItem(142).NumericValue;

        public virtual string EnergyUnits => ItemValues.GetItem(141).Description;

        public Dictionary<string, string> ItemData
            => ItemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public virtual double MeterDisplacement => 0.0d;
        public virtual string MeterModel => string.Empty;

        public virtual int MeterModelId => 0;
        public virtual double UncorrectedMultiplier => ItemValues.GetItem(92).NumericValue;

        public virtual double UncorrectedReading => ItemValues.GetItem(2).NumericValue;
        public virtual string UncorrectedUnits => ItemValues.GetItem(92).Description;
    }

    internal static class HighResVolumeHelpers
    {
        public static double GetHighResFractionalValue(double highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue, CultureInfo.InvariantCulture);
            var pointLocation = highResString.IndexOf(".", StringComparison.Ordinal);

            if (highResValue > 0 && pointLocation > -1)
            {
                var result = highResString.Substring(pointLocation, highResString.Length - pointLocation);

                return Convert.ToDouble(result);
            }

            return 0;
        }

        public static double GetHighResolutionItemValue(int lowResValue, double highResValue)
        {
            var fractional = GetHighResFractionalValue(highResValue);
            return lowResValue + fractional;
        }

        public static double GetHighResolutionValue(this IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return 0.0d;

            var items = itemValues as ItemValue[] ?? itemValues.ToArray();
            double? lowResValue = items?.GetItem(lowResItemNumber)?.NumericValue ?? 0;
            double? highResValue = items?.GetItem(highResItemNumber)?.NumericValue ?? 0;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static double JoinLowResHighResReading(double? lowResValue, double? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int) lowResValue.Value, highResValue.Value);
        }
    }
}