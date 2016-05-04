using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionGas.MASA.Models
{
    public class EvcQARun
    {
        public EvcQARun()
        {

        }

        public DateTime DateTime { get; set; }
        public string InstrumentType { get; set; }
        public List<VerificationTest> VerificationTests { get; set; };

        public class VerificationTest
        {
            public VerificationTest()
            {

            }

            public virtual PressureDetail Pressure { get; set; }
            public virtual TemperatureDetail Temperature { get; set; }
            public virtual VolumeDetail VolumeTest { get; set; }
            public virtual SuperFactorTest SuperFactorTest { get; set; }

            public class PressureDetail
            {
                public decimal GasPressure { get; set; }
                public decimal GasGauge { get; set; }
                public decimal AtmosphericGauge { get; set; }
                public decimal ActualFactor { get; set; }

                public decimal EvcPressure { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal PercentError { get; set; }
            }

            public class TemperatureDetail
            {
                public decimal GaugeTemperature { get; set; }
                public decimal EvcTemperature { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal ActualFactor { get; set; }
                public decimal PercentError { get; set; }
            }

            public class VolumeDetail
            {
                public int PulseACount { get; set; }
                public int PulseBCount { get; set; }
                public decimal AppliedInput { get; set; }
                public string DriveTypeDiscriminator { get; set; }            
                public decimal? UnCorrectedPercentError { get; set; }
                public decimal? CorrectedPercentError { get; set; }
                public int UncPulseCount { get; set; }
                public int CorPulseCount { get; set; }
                public decimal? TrueCorrected { get; set; }

                public string PulseASelect { get; set; }

                public string PulseBSelect { get; set; }
                public decimal EvcCorrected { get; set; }

                public decimal EvcUncorrected { get; set; }

                public decimal Corrected { get; set; }

                public decimal Uncorrected { get; set; }
                public string DriveRateDescription { get; set; }

                public string CorrectedMultiplierDescription { get; set; }
                public string UnCorrectedMultiplierDescription { get; set; }

            }
        }
    }
}
