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
            Low = 0,
            Medium = 1,
            High = 2
        }

        public TemperatureTest(Instrument instrument, Level level)
        {
            Instrument = instrument;
            Id = Guid.NewGuid();
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsTemperatureTest == true).ToList();
        }

        public Level TestLevel { get; set; }
        public double Gauge { get; set; }
        public double PercentError { get; set; }
        public double HasPassed { get; set; }

        public double? ActualFactor
        {
            get
            {
                switch (Units)
                {
                    case "K":
                    case "C":
                        return Math.Round((double) ((MetericTempCorrection + EvcBase)/(Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return Math.Round((double) ((TempCorrection + EvcBase)/(Gauge + TempCorrection)), 4);
                }

                return 0.00;
            }
        }

        [NotMapped]
        public Instrument Instrument { get; set; }
        
        [NotMapped]
        public string Units
        {
            get
            {
                return DescriptionValue((int)TempItems.Units);
            }
        }

        [NotMapped]
        public double? EvcBase
        {
            get
            {
                return NumericValue((int)TempItems.Base);
            }
        }

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
