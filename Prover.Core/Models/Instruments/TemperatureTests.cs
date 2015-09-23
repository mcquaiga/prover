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

        public TemperatureTest()
        {
            Items = Item.LoadItems(InstrumentType.MiniMax).Where(x => x.IsTemperatureTest == true).ToList();
        }

        public TemperatureTest(Temperature temp, InstrumentType type, Level level)
        {
            Temperature = temp;
            TemperatureId = temp.Id;
            Items = Item.LoadItems(type).Where(x => x.IsTemperatureTest == true).ToList();
            TestLevel = level;
            IsVolumeTestTemperature = TestLevel == Level.Low;
        }

        public Guid TemperatureId { get; set; }
        public Temperature Temperature { get; set; }

        public bool IsVolumeTestTemperature { get; set; }

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

        [NotMapped]
        public bool HasPassed => (PercentError < 1 && PercentError > -1);

        public double? ActualFactor
        {
            get
            {
                switch (Temperature.Units)
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (double)
                                    ((MetericTempCorrection + Temperature.EvcBase)/
                                     (Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (double) ((TempCorrection + Temperature.EvcBase)/(Gauge + TempCorrection)), 4);
                }

                return 0.00;
            }
        }

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
