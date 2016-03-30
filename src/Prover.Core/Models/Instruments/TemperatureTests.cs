using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class TemperatureTest : InstrumentTable
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        private enum TempItems
        {
            Gas = 26,
            Base = 34,
            Factor = 45,
            Units = 89,
            FixedFactor = 111
        }

        public TemperatureTest(Temperature temp, bool isVolumeTest = false, double defaultGauge = 0) : 
            base(temp.Instrument.Items.CopyItemsByFilter(x => x.IsTemperatureTest == true))
        {
            Temperature = temp;
            TemperatureId = temp.Id;
            IsVolumeTestTemperature = isVolumeTest;
            Gauge = defaultGauge;
        }

        public Guid TemperatureId { get; set; }
        public Temperature Temperature { get; set; }

        public bool IsVolumeTestTemperature { get; set; }

        public double Gauge { get; set; }
        public decimal? PercentError
        {
            get
            {
                if (EvcFactor == null) return null;
                return Math.Round((decimal) ((EvcFactor - ActualFactor)/ActualFactor)*100, 2);
            }
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }

        public decimal? ActualFactor
        {
            get
            {
                switch (Temperature.Units)
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (decimal)
                                    ((MetericTempCorrection + Temperature.EvcBase)/
                                     ((decimal)Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (decimal) ((TempCorrection + Temperature.EvcBase)/((decimal)Gauge + TempCorrection)), 4);
                }

                return 0;
            }
        }

        [NotMapped]
        public decimal? EvcReading
        {
            get { return Items.GetItem((int)TempItems.Gas).GetNumericValue(); }
        }

        [NotMapped]
        public decimal? EvcFactor
        {
            get { return Items.GetItem((int) TempItems.Factor).GetNumericValue(); }
        }
    }
}
