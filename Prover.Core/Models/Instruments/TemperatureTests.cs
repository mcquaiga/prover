using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class TemperatureTest : ItemsBase
    {
        private const double TempCorrection = 459.67;
        private const double MetericTempCorrection = 273.15;

        private enum TempItems
        {
            Gas = 26,
            Base = 34,
            Factor = 45,
            Units = 89,
            FixedFactor = 111
        }

        public enum Level
        {
            Low,
            Medium,
            High
        }

        public TemperatureTest(Instrument instrument, Level level)
        {
            Instrument = instrument;
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsTemperatureTest == true).ToList();
            TestLevel = level;
        }
        public Guid TemperatureId { get; set; }
        [ForeignKey("TemperatureId")]
        public virtual Temperature Temperature { get; set; }

        public Level TestLevel { get; set; }
        public double Gauge { get; set; }
        public double? PercentError
        {
            get
            {
                if (EvcFactor == null) return null;
                return Math.Round((double) ((EvcFactor - ActualFactor)/ActualFactor)*100, 2);
            }
        }
        public bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }
        public double? ActualFactor
        {
            get
            {
                switch (Instrument.Temperature.Units)
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (double)
                                    ((MetericTempCorrection + Instrument.Temperature.EvcBase)/
                                     (Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (double) ((TempCorrection + Instrument.Temperature.EvcBase)/(Gauge + TempCorrection)), 4);
                }

                return 0.00;
            }
        }


        [NotMapped]
        public Instrument Instrument { get; set; }

        [NotMapped]
        public double? EvcReading
        {
            get { return NumericValue((int) TempItems.Gas); }
        }

        [NotMapped]
        public double? EvcFactor
        {
            get { return NumericValue((int) TempItems.Factor); }
        }
    }
}
