using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class TemperatureTest
    {
        private const double TempCorrection = 459.67;
        private const double MetericTempCorrection = 273.15;
        private string _data = null;

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

        public TemperatureTest()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
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
        public ICollection<InstrumentValue> InstrumentValues { get; set; }
        
        [NotMapped]
        public string Units
        {
            get
            {
                return Instrument.DescriptionValue((int)TempItems.Units);
            }
        }

        [NotMapped]
        public double? EvcBase
        {
            get
            {
                var firstOrDefault = InstrumentValues.FirstOrDefault(x => x.Number == (int)TempItems.Base);
                if (firstOrDefault != null)
                    return firstOrDefault.Value;
                return null;
            }
        }

        [NotMapped]
        public double? EvcReading
        {
            get
            {
                var firstOrDefault = InstrumentValues.FirstOrDefault(x => x.Number == (int)TempItems.Gas);
                if (firstOrDefault != null)
                    return firstOrDefault.Value;
                return null;
            }
        }

        [NotMapped]
        public double? EvcFactor
        {
            get
            {
                var firstOrDefault = InstrumentValues.FirstOrDefault(x => x.Number == (int)TempItems.Factor);
                if (firstOrDefault != null)
                    return firstOrDefault.Value;
                return null;
            }
        }

        public virtual Temperature Temp { get; set; }

        public string Data
        {
            get { return JsonConvert.SerializeObject(InstrumentValues); }
            set { _data = value; }
        }

    }
}
