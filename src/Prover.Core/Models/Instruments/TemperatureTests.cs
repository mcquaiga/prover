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
    public class TemperatureTest : ItemsBase
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

        public enum Level
        {
            Low,
            Medium,
            High
        }

        public enum SuggestedGauges
        {
            Low = 32,
            Medium = 60,
            High = 90
        }

        public TemperatureTest()
        {
            Items = Item.LoadItems(InstrumentType.MiniMax).Where(x => x.IsTemperatureTest == true).ToList();
        }

        public TemperatureTest(Temperature temp, InstrumentType type, Level level) : this()
        {
            Temperature = temp;
            TemperatureId = temp.Id;
            TestLevel = level;
            IsVolumeTestTemperature = TestLevel == Level.Low;
            SetDefaultGauge(level);
        }

        public Guid TemperatureId { get; set; }
        public Temperature Temperature { get; set; }

        public bool IsVolumeTestTemperature { get; set; }

        public Level TestLevel { get; set; }
        public decimal Gauge { get; set; }
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
                                     (Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (decimal) ((TempCorrection + Temperature.EvcBase)/(Gauge + TempCorrection)), 4);
                }

                return 0;
            }
        }
        
        public void SetDefaultGauge(Level templevel)
        {
            switch (templevel)
            {
                case Level.Low:
                    Gauge = 32;
                    break;
                case Level.Medium:
                    Gauge = 60;
                    break;
                case Level.High:
                    Gauge = 90;
                    break;
            }
        }

        [NotMapped]
        public decimal? EvcReading
        {
            get { return NumericValue((int) TempItems.Gas); }
        }

        [NotMapped]
        public decimal? EvcFactor
        {
            get { return NumericValue((int) TempItems.Factor); }
        }
    }
}
