using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{


    public class PressureTest : InstrumentTable
    {
        private int GAS_PRESSURE = 8;
        private int PRESSURE_FACTOR = 44;
        private int UNSQR_FACTOR = 47;
        
        public PressureTest(Pressure pressure, bool isVolumeTest = false, decimal defaultGauge = 0) : 
            base(pressure.Instrument.Items.CopyItemsByFilter(x => x.IsPressureTest == true))
        {
            Pressure = pressure;
            PressureId = pressure.Id;
            IsVolumeTestPressure = isVolumeTest;
            GasGauge = defaultGauge;
            AtmosphericGauge = 0;
        }

        public Guid PressureId { get; set; }
        public Pressure Pressure { get; set; }
        public bool IsVolumeTestPressure { get; private set; }

        public decimal? GasGauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }
        public decimal? PercentError
        {
            get
            {
                if (EvcFactor == null) return null;
                return Math.Round((decimal)((EvcFactor - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public decimal? ActualFactor
        {
            get
            {
                if (Pressure.EvcBase == 0) return 0;

                decimal? result;

                switch (Pressure.TransducerType)
                {
                    case TransducerType.Gauge:
                        result = (GasGauge + Pressure.EvcAtmospheric) / Pressure.EvcBase;
                        break;

                    case TransducerType.Absolute:
                        result = (GasGauge + AtmosphericGauge) / Pressure.EvcBase;
                        break;
                    default:
                        result = 0;
                        break;
                }

                return result.HasValue ? decimal.Round(result.Value, 4) : result;
            }
        }

        /*
        ** EVC PRopertes
        */
        [NotMapped]
        public decimal? EvcGasPressure
        {
            get
            {
                return Items.GetItem(GAS_PRESSURE).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcFactor
        {
            get
            {
                return Items.GetItem(PRESSURE_FACTOR).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcUnsqrFactor
        {
            get
            {
                return Pressure.Items.GetItem(UNSQR_FACTOR).GetNumericValue();
            }
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }

        //public void SetDefaultGauge(PressureLevel level)
        //{
        //    int percent;

        //    switch (level)
        //    {
        //        case PressureLevel.Low:
        //            percent = 20;
        //            break;
        //        case PressureLevel.Medium:
        //            percent = 50;
        //            break;
        //        case PressureLevel.High:
        //            percent = 80;
        //            break;
        //        default:
        //            percent = 0;
        //            break;
        //    }

        //    GasGauge = ((decimal)percent / 100) * Pressure.EvcPressureRange;
        //}
    }
}
