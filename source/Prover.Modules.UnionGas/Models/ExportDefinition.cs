using System;
using System.Collections.Generic;

namespace Prover.Modules.UnionGas.Models
{
    [Serializable]
    public class EvcQARun
    {
        public DateTimeOffset DateTime { get; set; }

        //Site Information
        public string InstrumentType { get; set; } //MiniMax, MiniAT, EC350
        public string InstrumentComposition { get; set; } //Comp. - Values = PT, T, P

        public string CompanyNumber { get; set; }
        public string SerialNumber { get; set; }
        public string DriveType { get; set; } // Rotary, Mechanical
        public string MeterType { get; set; } // Specific to Rotary DriveType

        public decimal MeterDisplacement { get; set; } //Mechanical Drive Rate/Meter Displacement
        public decimal FirmwareVersion { get; set; } // Firm. Item 122

        public string InstrumentData { get; set; } //Instrument Item File downloaded at the start of the test
        public string ConfirmedStatus { get; set; } //CONFIRMED STATUS - Pass/Fail

        //Pressure Info
        public PressureHeader PressureInfo { get; set; }

        //Temperature Info
        public TemperatureHeader TemperatureInfo { get; set; }

        //Supercompress. Info - Gas Composition
        public SuperFactorHeader SuperFactorInfo { get; set; }

        //Volume Info
        public VolumeHeader VolumeInfo { get; set; }

        public List<VerificationTest> VerificationTests { get; set; }

        public class PressureHeader
        {
            public string TransducerType { get; set; } // A or G
            public string PressureUnits { get; set; } //PSIA or PSIG
            public decimal BasePressure { get; set; } // (3,2)
            public decimal PressureRange { get; set; } //0 - PressureRange (6,2)
            public decimal ProgrammedAtmosphericPressure { get; set; } // (3,2)
        }

        public class TemperatureHeader
        {
            public string TemperatureRange { get; set; } // -40 to 170
            public string TemperatureUnits { get; set; } // F or C
            public decimal BaseTemperature { get; set; } // (3, 2)
        }

        public class SuperFactorHeader
        {
            public decimal SpecGr { get; set; } // SG - (4,4)
            public decimal CO2 { get; set; } // (4, 4)
            public decimal N2 { get; set; } // (4, 4)
            public string FPVTable { get; set; } //FPV = NX-19
        }

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

        public class VerificationTest
        {
            public PressureTest Pressure { get; set; }
            public TemperatureTest Temperature { get; set; }
            public VolumeTest Volume { get; set; }
            public SuperFactorTest SuperFactor { get; set; }

            public class PressureTest
            {
                public decimal PercentError { get; set; } //Only value displayed on Certificate

                public decimal GasPressure { get; set; } // (4, 2)
                public decimal GaugePressure { get; set; } // (4, 2)
                public decimal AtmosphericGauge { get; set; } // (4, 2)
                public decimal ActualFactor { get; set; } // (4, 4)
                public decimal EvcGasPressure { get; set; } // (4, 2)
                public decimal EvcPressureFactor { get; set; } // (4, 4)
            }

            public class TemperatureTest
            {
                public decimal PercentError { get; set; } //Only value displayed on Certificate
                public decimal GaugeTemperature { get; set; } // (4, 2) - 90.0, 60.0, 32.0
                public decimal EvcTemperature { get; set; } // (4, 2)
                public decimal EvcFactor { get; set; } // (4, 4)
                public decimal ActualFactor { get; set; } // (4, 4)
            }

            public class VolumeTest
            {
                public decimal? UnCorrectedPercentError { get; set; } //Only value displayed on Certificate - (3, 2)
                public decimal? CorrectedPercentError { get; set; } //Only value displayed on Certificate - (3, 2)

                public int PulseACount { get; set; }
                public int PulseBCount { get; set; }

                public decimal AppliedInput { get; set; } // (8, 4)
                public decimal TrueCorrected { get; set; } // (8, 4)

                public int UncPulseCount { get; set; }
                public int CorPulseCount { get; set; }

                public decimal? EvcCorrected { get; set; } // (8, 4)
                public decimal? EvcUncorrected { get; set; } // (8, 4)
            }

            public class SuperFactorTest
            {
                public decimal PercentError { get; set; } // (3, 2) - Only value displayed on Certificate

                public decimal GaugeTemp { get; set; } // (4, 2) - Same value as TemperatureTest.GaugeTemperature value
                public decimal GaugePressure { get; set; } // (4, 2) - Same value as PressureTest.GaugePressure value

                public decimal EVCUnsqrFactor { get; set; } // (4, 4)
                public decimal EvcFactor { get; set; } // (4, 4)
                public decimal ActualFactor { get; set; } // (4, 4) 
            }
        }
    }
}