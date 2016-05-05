using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionGas.MASA.Models
{
    public class EvcQARun
    {
        public DateTimeOffset DateTime { get; set; }

        //Site Information
        public string InstrumentType { get; set; } //MiniMax, MiniAT, EC350
        public string InstrumentComposition { get; set; } //Comp. - Values = PT, T, P

        public string CompanyNumber { get; set; }
        public string SerialNumber { get; set; }
        public string DriveType { get; set; }
        public string MeterType { get; set; } // Specific to Rotary DriveType

        public decimal MeterDisplacement { get; set; } //Mechanical Drive Rate/Meter Displacement
        public decimal FirmwareVersion { get; set; } // Firm. Item 122

        public string InstrumentData { get; set; } //Instrument Item File downloaded at the start of the test
        public string ConfirmedStatus { get; set; } //CONFIRMED STATUS - Pass/Fail

        //Pressure Info
        public class PressureHeader
        {
            public string TransducerType { get; set; } // A or G
            public string PressureUnits { get; set; } //PSIA or PSIG
            public decimal BasePressure { get; set; }
            public decimal PressureRange { get; set; } //0 - PressureRange
            public decimal ProgrammedAtmosphericPressure { get; set; } //
        }

        //Temperature Info
        public class TemperatureHeader
        {
            public string TemperatureRange { get; set; } // -40 to 170
            public string TemperatureUnits { get; set; } // F or C
            public decimal BaseTemperature { get; set; } 
        }

        //Supercompress. Info - Gas Composition
        public class SuperFactorHeader
        {
            public decimal SpecGr { get; set; } // SG
            public decimal CO2 { get; set; }
            public decimal N2 { get; set; }
            public string FPVTable { get; set; } //FPV = NX-19
        }

        //Volume Info
        public class VolumeHeader
        {
            public string PulseASelect { get; set; }
            public string PulseBSelect { get; set; }
            public string DriveRateDescription { get; set; }

            public int CorrectedMultiplierValue { get; set; } //Numeric value for calculations - 1, 10, 100, 1000
            public string CorrectedMultiplierDescription { get; set; } // Description - CuFTx10

            public int UncorrectedMultiplierValue { get; set; } //Numeric value for calculations - 1, 10, 100, 1000
            public string UncorrectedMultiplierDescription { get; set; } // Description - CuFTx10
        }

        public List<VerificationTest> VerificationTests { get; set; }

        public class VerificationTest
        {
            public PressureTest Pressure { get; set; }
            public TemperatureTest Temperature { get; set; }
            public VolumeTest Volume { get; set; }
            public SuperFactorTest SuperFactor { get; set; }

            public class PressureTest
            {
                public decimal PercentError { get; set; } //Only value displayed on Certificate

                public decimal GasPressure { get; set; }
                public decimal GaugePressure { get; set; }
                public decimal AtmosphericGauge { get; set; }
                public decimal ActualFactor { get; set; }
                public decimal EvcGasPressure { get; set; }
                public decimal EvcPressureFactor { get; set; }
            }

            public class TemperatureTest
            {
                public decimal PercentError { get; set; } //Only value displayed on Certificate
                public decimal GaugeTemperature { get; set; }
                public decimal EvcTemperature { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal ActualFactor { get; set; }
            }

            public class VolumeTest
            {
                public decimal UnCorrectedPercentError { get; set; } //Only value displayed on Certificate
                public decimal CorrectedPercentError { get; set; } //Only value displayed on Certificate

                public int PulseACount { get; set; }
                public int PulseBCount { get; set; }
                public decimal AppliedInput { get; set; }         

                public int UncPulseCount { get; set; }
                public int CorPulseCount { get; set; }
                public decimal TrueCorrected { get; set; }
                public decimal EvcCorrected { get; set; }
                public decimal EvcUncorrected { get; set; }

                public decimal Corrected { get; set; }
                public decimal Uncorrected { get; set; }
            }

            public class SuperFactorTest
            {
                public decimal PercentError { get; set; } //Only value displayed on Certificate

                public decimal GaugeTemp { get; set; }
                public decimal GaugePressure { get; set; }

                public decimal EVCUnsqrFactor { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal ActualFactor { get; set; }
            }
        }
    }
}
