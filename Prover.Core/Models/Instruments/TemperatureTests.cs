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
            Id = Guid.NewGuid();
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsTemperatureTest == true).ToList();
            TestLevel = level;
        }

        public Level TestLevel { get; set; }
        public double Gauge { get; set; }

        public double PercentError { get; set; }
        
        public double HasPassed { get; set; }

        public double? ActualFactor
        {
            get
            {
                switch (Instrument.Temperature.Units)
                {
                    case "K":
                    case "C":
                        return Math.Round((double) ((MetericTempCorrection + Instrument.Temperature.EvcBase)/(Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return Math.Round((double)((TempCorrection + Instrument.Temperature.EvcBase) / (Gauge + TempCorrection)), 4);
                }

                return 0.00;
            }
        }

        [NotMapped]
        public Instrument Instrument { get; set; }
       

        [NotMapped]
        public double? EvcReading
        {
            get
            {
                return NumericValue((int)TempItems.Gas);
            }
        }

        [NotMapped]
        public double? EvcFactor
        {
            get
            {
                return NumericValue((int)TempItems.Factor);
            }
        }

        public virtual Temperature Temp { get; set; }
    }
}
