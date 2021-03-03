using System;
using System.Collections.Generic;

namespace UnionGas.MASA.Models {

	[Serializable]
	public class EvcQARun {

		#region Properties

		public string BarCodeNumber { get; set; }

		public string CompanyNumber { get; set; }

		public string ConfirmedStatus { get; set; }

		public DateTimeOffset DateTime { get; set; }

		public string DriveType { get; set; }

		public decimal FirmwareVersion { get; set; }

		public string InstrumentComposition { get; set; }

		public string InstrumentData { get; set; }

		//Site Information
		public string InstrumentType { get; set; } //MiniMax, MiniAT, EC350

		public decimal MeterDisplacement { get; set; }

		// Rotary, Mechanical
		public string MeterType { get; set; }

		//Pressure Info
		public PressureHeader PressureInfo { get; set; }

		//Comp. - Values = PT, T, P
		public string SerialNumber { get; set; }

		// Specific to Rotary DriveType

		//Mechanical Drive Rate/Meter Displacement
		// Firm. Item 122

		//Supercompress. Info - Gas Composition
		public SuperFactorHeader SuperFactorInfo { get; set; }

		//Instrument Item File downloaded at the start of the test
		//CONFIRMED STATUS - Pass/Fail
		//Temperature Info
		public TemperatureHeader TemperatureInfo { get; set; }

		public List<VerificationTest> VerificationTests { get; set; }

		//Volume Info
		public VolumeHeader VolumeInfo { get; set; }

		#endregion

		#region Classes

		public class PressureHeader {

			#region Properties

			public decimal BasePressure { get; set; }

			// (3,2)
			public decimal PressureRange { get; set; }

			public string PressureUnits { get; set; }

			//PSIA or PSIG
			//0 - PressureRange (6,2)
			public decimal ProgrammedAtmosphericPressure { get; set; }

			public string TransducerType { get; set; }

			#endregion

			// A or G (3,2)
		}

		public class SuperFactorHeader {

			#region Properties

			public decimal CO2 { get; set; }

			public string FPVTable { get; set; }

			// (4, 4)
			public decimal N2 { get; set; }

			public decimal SpecGr { get; set; }

			#endregion

			// SG - (4,4)
			// (4, 4)
			//FPV = NX-19
		}

		public class TemperatureHeader {

			#region Properties

			public decimal BaseTemperature { get; set; }

			public string TemperatureRange { get; set; } // -40 to 170

			public string TemperatureUnits { get; set; }

			#endregion

			// F or C (3, 2)
		}

		public class VerificationTest {

			#region Properties

			public PressureTest Pressure { get; set; }

			public SuperFactorTest SuperFactor { get; set; }

			public TemperatureTest Temperature { get; set; }

			public VolumeTest Volume { get; set; }

			#endregion

			#region Classes

			public class PressureTest {

				#region Properties

				public decimal ActualFactor { get; set; }

				public decimal AtmosphericGauge { get; set; }

				// (4, 2) (4, 4)
				public decimal EvcGasPressure { get; set; }

				// (4, 2)
				public decimal EvcPressureFactor { get; set; }

				public decimal GasPressure { get; set; }

				// (4, 2)
				public decimal GaugePressure { get; set; }

				public decimal PercentError { get; set; }

				#endregion

				//Only value displayed on Certificate

				// (4, 2) (4, 4)
			}

			public class SuperFactorTest {

				#region Properties

				public decimal ActualFactor { get; set; }

				public decimal EvcFactor { get; set; }

				public decimal EVCUnsqrFactor { get; set; }

				public decimal GaugePressure { get; set; }

				public decimal GaugeTemp { get; set; }

				public decimal PercentError { get; set; }

				#endregion

				// (3, 2) - Only value displayed on Certificate

				// (4, 2) - Same value as TemperatureTest.GaugeTemperature value (4, 2) - Same value
				// as PressureTest.GaugePressure value

				// (4, 4) (4, 4) (4, 4)
			}

			public class TemperatureTest {

				#region Properties

				public decimal ActualFactor { get; set; }

				public decimal EvcFactor { get; set; }

				public decimal EvcTemperature { get; set; }

				public decimal GaugeTemperature { get; set; }

				public decimal PercentError { get; set; }

				#endregion

				//Only value displayed on Certificate
				// (4, 2) - 90.0, 60.0, 32.0
				// (4, 2)
				// (4, 4)
				// (4, 4)
			}

			public class VolumeTest {

				#region Properties

				public decimal AppliedInput { get; set; }

				public int CorPulseCount { get; set; }

				public decimal? CorrectedPercentError { get; set; }

				public decimal? EvcCorrected { get; set; }

				// (8, 4)
				public decimal? EvcUncorrected { get; set; }

				public int PulseACount { get; set; }

				//Only value displayed on Certificate - (3, 2)
				public int PulseBCount { get; set; }

				// (8, 4)
				public decimal TrueCorrected { get; set; }

				public decimal? UnCorrectedPercentError { get; set; } //Only value displayed on Certificate - (3, 2)

																	  // (8, 4)

				public int UncPulseCount { get; set; }

				#endregion

				// (8, 4)
			}

			#endregion
		}

		public class VolumeHeader {

			#region Properties

			public string CorrectedMultiplierDescription { get; set; }

			public int CorrectedMultiplierValue { get; set; }

			public string DriveRateDescription { get; set; }

			public string PulseASelect { get; set; }

			public string PulseBSelect { get; set; }

			//Numeric value for calculations - 1, 10, 100, 1000
			// Description - CuFTx10

			public string UncorrectedMultiplierDescription { get; set; }

			public int UncorrectedMultiplierValue { get; set; }

			#endregion

			//Numeric value for calculations - 1, 10, 100, 1000
			// Description - CuFTx10
		}

		#endregion
	}
}