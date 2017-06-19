using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Enums;

namespace Prover.GUI.Modules.QAProver.Screens.InstrumentInfo.Designer
{
    public class DesignTimeInstrumentInfoViewModel
    {
        public DesignTimeInstrumentInfoViewModel()
        {
            var items = new List<ItemValue>()
            {
                new ItemValue(new ItemMetadata() {Number = 62}, "200000"),
                new ItemValue(new ItemMetadata() {Number = 200}, "200000"),
                new ItemValue(new ItemMetadata() {Number = 201}, "300000"),
            };

            Instrument = new Instrument(HoneywellInstrumentTypes.MiniAt, items);
        }

        public Instrument Instrument { get; set; }

        public string CorrectorType
        {
            get
            {
                switch (Instrument.CompositionType)
                {
                    case EvcCorrectorType.PTZ:
                        return "PTZ";
                    case EvcCorrectorType.P:
                        return "P";
                    default:
                        return "T";
                }
            }
        }

        public string BasePressure
            =>
            $"{Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}"
            ;

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public string TestDatePretty => $"{Instrument.TestDateTime:MMMM d, yyyy h:mm tt}";

        public string JobIdDisplay => !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

        public bool EventLogChecked { get; set; }     

        public bool CommPortChecked { get; set; }

        #region Data

        private readonly string _json = @"{
  ""Certificate"": null,
  ""Client"": null,
  ""VerificationTests"": [
    {
      ""TestNumber"": 1,
      ""InstrumentId"": ""8d1ee376-f0a3-4119-b649-aeb20e2add49"",
      ""PressureTest"": {
        ""VerificationTestId"": ""62a8fb9f-5683-410a-8f87-d220f14a3794"",
        ""GasPressure"": 50.00,
        ""GasGauge"": 50.00,
        ""AtmosphericGauge"": 0.00,
        ""PercentError"": 0.12,
        ""ActualFactor"": 3.3944,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""8\"":\""   50.06\"",\""44\"":\""  3.3984\"",\""47\"":\""  1.0025\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   50.06"",
            ""Metadata"": {
              ""Number"": 8,
              ""Code"": ""GAS_PRESS"",
              ""ShortDescription"": ""Gas Press"",
              ""LongDescription"": ""Gas Pressure"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 50.06,
            ""Description"": ""50.06""
          },
          {
            ""RawValue"": ""  3.3984"",
            ""Metadata"": {
              ""Number"": 44,
              ""Code"": ""PRESS_FACTOR"",
              ""ShortDescription"": ""Press Factor"",
              ""LongDescription"": ""Pressure Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 3.3984,
            ""Description"": ""3.3984""
          },
          {
            ""RawValue"": ""  1.0025"",
            ""Metadata"": {
              ""Number"": 47,
              ""Code"": ""UNSQRD_SUPER_FACTOR"",
              ""ShortDescription"": ""Unsqrd. Super Factor"",
              ""LongDescription"": ""Unsquared Supercompresibility Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0025,
            ""Description"": ""1.0025""
          }
        ],
        ""Id"": ""62a8fb9f-5683-410a-8f87-d220f14a3794""
      },
      ""TemperatureTest"": {
        ""VerificationTestId"": ""62a8fb9f-5683-410a-8f87-d220f14a3794"",
        ""Gauge"": 60.0,
        ""PercentError"": -0.01,
        ""ActualFactor"": 1.0,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""26\"":\""   60.07\"",\""35\"":\"" -0.1740\"",\""45\"":\""  0.9999\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   60.07"",
            ""Metadata"": {
              ""Number"": 26,
              ""Code"": ""GAS_TEMP"",
              ""ShortDescription"": ""Gas Temperat"",
              ""LongDescription"": ""EVC Gas Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 60.07,
            ""Description"": ""60.07""
          },
          {
            ""RawValue"": "" -0.1740"",
            ""Metadata"": {
              ""Number"": 35,
              ""Code"": """",
              ""ShortDescription"": ""Temperature"",
              ""LongDescription"": ""EVC Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": -0.1740,
            ""Description"": ""-0.1740""
          },
          {
            ""RawValue"": ""  0.9999"",
            ""Metadata"": {
              ""Number"": 45,
              ""Code"": ""TEMP_FACTOR"",
              ""ShortDescription"": ""Temperature Factor"",
              ""LongDescription"": ""Temperature Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.9999,
            ""Description"": ""0.9999""
          }
        ],
        ""Id"": ""62a8fb9f-5683-410a-8f87-d220f14a3794""
      },
      ""VolumeTest"": null,
      ""SuperFactorTest"": {
        ""SuperFactorCaculations"": null,
        ""GaugeTemp"": 60.0,
        ""GaugePressure"": 50.00,
        ""EvcUnsqrFactor"": 1.0025,
        ""ActualFactor"": 1.0035,
        ""SuperFactorSquared"": 1.00701225,
        ""PercentError"": -0.10,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""53\"":\""  0.5778\"",\""54\"":\""  1.4260\"",\""55\"":\""  0.6970\"",\""110\"":\""       0\""}"",
        ""Items"": [
          {
            ""RawValue"": ""  0.5778"",
            ""Metadata"": {
              ""Number"": 53,
              ""Code"": ""SPEC_GR"",
              ""ShortDescription"": ""Specific Gravity"",
              ""LongDescription"": ""Specific Gravity"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.5778,
            ""Description"": ""0.5778""
          },
          {
            ""RawValue"": ""  1.4260"",
            ""Metadata"": {
              ""Number"": 54,
              ""Code"": ""N2"",
              ""ShortDescription"": ""% N2"",
              ""LongDescription"": ""% N2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.4260,
            ""Description"": ""1.4260""
          },
          {
            ""RawValue"": ""  0.6970"",
            ""Metadata"": {
              ""Number"": 55,
              ""Code"": ""CO2"",
              ""ShortDescription"": ""% CO2"",
              ""LongDescription"": ""% CO2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.6970,
            ""Description"": ""0.6970""
          },
          {
            ""RawValue"": ""       0"",
            ""Metadata"": {
              ""Number"": 110,
              ""Code"": ""FIXED_SUPER_FACTOR"",
              ""ShortDescription"": ""F Super Factor"",
              ""LongDescription"": ""Fixed Super Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": [
                {
                  ""Id"": 0,
                  ""Description"": ""Live"",
                  ""Value"": 0.0
                },
                {
                  ""Id"": 1,
                  ""Description"": ""Fixed"",
                  ""Value"": 1.0
                }
              ]
            },
            ""NumericValue"": 0.0,
            ""Description"": ""Live""
          }
        ],
        ""Id"": ""c3799499-a7e6-4dea-8462-3294078ff405""
      },
      ""HasPassed"": true,
      ""Id"": ""62a8fb9f-5683-410a-8f87-d220f14a3794""
    },
    {
      ""TestNumber"": 0,
      ""InstrumentId"": ""8d1ee376-f0a3-4119-b649-aeb20e2add49"",
      ""PressureTest"": {
        ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
        ""GasPressure"": 80.00,
        ""GasGauge"": 80.00,
        ""AtmosphericGauge"": 0.00,
        ""PercentError"": 0.27,
        ""ActualFactor"": 5.4311,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""8\"":\""   80.22\"",\""44\"":\""  5.4457\"",\""47\"":\""  1.0056\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   80.22"",
            ""Metadata"": {
              ""Number"": 8,
              ""Code"": ""GAS_PRESS"",
              ""ShortDescription"": ""Gas Press"",
              ""LongDescription"": ""Gas Pressure"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 80.22,
            ""Description"": ""80.22""
          },
          {
            ""RawValue"": ""  5.4457"",
            ""Metadata"": {
              ""Number"": 44,
              ""Code"": ""PRESS_FACTOR"",
              ""ShortDescription"": ""Press Factor"",
              ""LongDescription"": ""Pressure Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 5.4457,
            ""Description"": ""5.4457""
          },
          {
            ""RawValue"": ""  1.0056"",
            ""Metadata"": {
              ""Number"": 47,
              ""Code"": ""UNSQRD_SUPER_FACTOR"",
              ""ShortDescription"": ""Unsqrd. Super Factor"",
              ""LongDescription"": ""Unsquared Supercompresibility Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0056,
            ""Description"": ""1.0056""
          }
        ],
        ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
      },
      ""TemperatureTest"": {
        ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
        ""Gauge"": 32.0,
        ""PercentError"": -0.02,
        ""ActualFactor"": 1.0569,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""26\"":\""   32.11\"",\""35\"":\"" -0.1740\"",\""45\"":\""  1.0567\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   32.11"",
            ""Metadata"": {
              ""Number"": 26,
              ""Code"": ""GAS_TEMP"",
              ""ShortDescription"": ""Gas Temperat"",
              ""LongDescription"": ""EVC Gas Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 32.11,
            ""Description"": ""32.11""
          },
          {
            ""RawValue"": "" -0.1740"",
            ""Metadata"": {
              ""Number"": 35,
              ""Code"": """",
              ""ShortDescription"": ""Temperature"",
              ""LongDescription"": ""EVC Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": -0.1740,
            ""Description"": ""-0.1740""
          },
          {
            ""RawValue"": ""  1.0567"",
            ""Metadata"": {
              ""Number"": 45,
              ""Code"": ""TEMP_FACTOR"",
              ""ShortDescription"": ""Temperature Factor"",
              ""LongDescription"": ""Temperature Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0567,
            ""Description"": ""1.0567""
          }
        ],
        ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
      },
      ""VolumeTest"": {
        ""PulseACount"": 5,
        ""PulseBCount"": 10,
        ""AppliedInput"": 100.00,
        ""DriveType"": {
          ""Energy"": {
            ""HasPassed"": true,
            ""PercentError"": -0.23,
            ""EvcEnergy"": 58.0,
            ""EnergyUnits"": ""Therms"",
            ""ActualEnergy"": 58.136
          },
          ""Discriminator"": ""Mechanical"",
          ""HasPassed"": true
        },
        ""TestInstrumentData"": ""{\""0\"":\""00000187\"",\""2\"":\""00000328\"",\""113\"":\""187.2885\"",\""140\"":\""00033347\""}"",
        ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
        ""AfterTestItems"": [
          {
            ""RawValue"": ""00000187"",
            ""Metadata"": {
              ""Number"": 0,
              ""Code"": ""COR_VOL"",
              ""ShortDescription"": ""Corrected Vol"",
              ""LongDescription"": ""Corrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 187.0,
            ""Description"": ""187""
          },
          {
            ""RawValue"": ""00000328"",
            ""Metadata"": {
              ""Number"": 2,
              ""Code"": ""UNCOR_VOL"",
              ""ShortDescription"": ""UnCor Vol"",
              ""LongDescription"": ""Uncorrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 328.0,
            ""Description"": ""328""
          },
          {
            ""RawValue"": ""187.2885"",
            ""Metadata"": {
              ""Number"": 113,
              ""Code"": ""HIGH_RES_CORRECTED"",
              ""ShortDescription"": ""High Res COR"",
              ""LongDescription"": ""High Resolution Corrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 187.2885,
            ""Description"": ""187.2885""
          },
          {
            ""RawValue"": ""00033347"",
            ""Metadata"": {
              ""Number"": 140,
              ""Code"": ""ENERGY"",
              ""ShortDescription"": ""Energy"",
              ""LongDescription"": ""Energy"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 33347.0,
            ""Description"": ""33347""
          }
        ],
        ""UnCorrectedPercentError"": 0.0,
        ""TrueUncorrected"": 1000.00,
        ""CorrectedPercentError"": -0.08,
        ""CorrectedHasPassed"": true,
        ""UnCorrectedHasPassed"": true,
        ""HasPassed"": true,
        ""PercentError"": null,
        ""ActualFactor"": null,
        ""UncPulseCount"": 10,
        ""CorPulseCount"": 5,
        ""UnCorPulsesPassed"": true,
        ""CorPulsesPassed"": true,
        ""TrueCorrected"": 5818.460776016241600000,
        ""EvcCorrected"": 5813.6000,
        ""EvcUncorrected"": 1000.0,
        ""DriveTypeDiscriminator"": ""Mechanical"",
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""InstrumentData"": ""{\""0\"":\""00000181\"",\""2\"":\""00000318\"",\""113\"":\""181.4749\"",\""140\"":\""00033289\""}"",
        ""Items"": [
          {
            ""RawValue"": ""00000181"",
            ""Metadata"": {
              ""Number"": 0,
              ""Code"": ""COR_VOL"",
              ""ShortDescription"": ""Corrected Vol"",
              ""LongDescription"": ""Corrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 181.0,
            ""Description"": ""181""
          },
          {
            ""RawValue"": ""00000318"",
            ""Metadata"": {
              ""Number"": 2,
              ""Code"": ""UNCOR_VOL"",
              ""ShortDescription"": ""UnCor Vol"",
              ""LongDescription"": ""Uncorrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 318.0,
            ""Description"": ""318""
          },
          {
            ""RawValue"": ""181.4749"",
            ""Metadata"": {
              ""Number"": 113,
              ""Code"": ""HIGH_RES_CORRECTED"",
              ""ShortDescription"": ""High Res COR"",
              ""LongDescription"": ""High Resolution Corrected Volume"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 181.4749,
            ""Description"": ""181.4749""
          },
          {
            ""RawValue"": ""00033289"",
            ""Metadata"": {
              ""Number"": 140,
              ""Code"": ""ENERGY"",
              ""ShortDescription"": ""Energy"",
              ""LongDescription"": ""Energy"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": true,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 33289.0,
            ""Description"": ""33289""
          }
        ],
        ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
      },
      ""SuperFactorTest"": {
        ""SuperFactorCaculations"": null,
        ""GaugeTemp"": 32.0,
        ""GaugePressure"": 80.00,
        ""EvcUnsqrFactor"": 1.0056,
        ""ActualFactor"": 1.0068,
        ""SuperFactorSquared"": 1.01364624,
        ""PercentError"": -0.12,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""53\"":\""  0.5778\"",\""54\"":\""  1.4260\"",\""55\"":\""  0.6970\"",\""110\"":\""       0\""}"",
        ""Items"": [
          {
            ""RawValue"": ""  0.5778"",
            ""Metadata"": {
              ""Number"": 53,
              ""Code"": ""SPEC_GR"",
              ""ShortDescription"": ""Specific Gravity"",
              ""LongDescription"": ""Specific Gravity"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.5778,
            ""Description"": ""0.5778""
          },
          {
            ""RawValue"": ""  1.4260"",
            ""Metadata"": {
              ""Number"": 54,
              ""Code"": ""N2"",
              ""ShortDescription"": ""% N2"",
              ""LongDescription"": ""% N2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.4260,
            ""Description"": ""1.4260""
          },
          {
            ""RawValue"": ""  0.6970"",
            ""Metadata"": {
              ""Number"": 55,
              ""Code"": ""CO2"",
              ""ShortDescription"": ""% CO2"",
              ""LongDescription"": ""% CO2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.6970,
            ""Description"": ""0.6970""
          },
          {
            ""RawValue"": ""       0"",
            ""Metadata"": {
              ""Number"": 110,
              ""Code"": ""FIXED_SUPER_FACTOR"",
              ""ShortDescription"": ""F Super Factor"",
              ""LongDescription"": ""Fixed Super Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": [
                {
                  ""Id"": 0,
                  ""Description"": ""Live"",
                  ""Value"": 0.0
                },
                {
                  ""Id"": 1,
                  ""Description"": ""Fixed"",
                  ""Value"": 1.0
                }
              ]
            },
            ""NumericValue"": 0.0,
            ""Description"": ""Live""
          }
        ],
        ""Id"": ""3bb137bc-2443-49c3-92e9-3a7fbaf6ab4b""
      },
      ""HasPassed"": true,
      ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
    },
    {
      ""TestNumber"": 2,
      ""InstrumentId"": ""8d1ee376-f0a3-4119-b649-aeb20e2add49"",
      ""PressureTest"": {
        ""VerificationTestId"": ""fe426ad0-a293-434a-bbf9-e3b8e7381429"",
        ""GasPressure"": 20.00,
        ""GasGauge"": 20.00,
        ""AtmosphericGauge"": 0.00,
        ""PercentError"": 0.05,
        ""ActualFactor"": 1.3578,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""8\"":\""   20.01\"",\""44\"":\""  1.3585\"",\""47\"":\""  1.0003\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   20.01"",
            ""Metadata"": {
              ""Number"": 8,
              ""Code"": ""GAS_PRESS"",
              ""ShortDescription"": ""Gas Press"",
              ""LongDescription"": ""Gas Pressure"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 20.01,
            ""Description"": ""20.01""
          },
          {
            ""RawValue"": ""  1.3585"",
            ""Metadata"": {
              ""Number"": 44,
              ""Code"": ""PRESS_FACTOR"",
              ""ShortDescription"": ""Press Factor"",
              ""LongDescription"": ""Pressure Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.3585,
            ""Description"": ""1.3585""
          },
          {
            ""RawValue"": ""  1.0003"",
            ""Metadata"": {
              ""Number"": 47,
              ""Code"": ""UNSQRD_SUPER_FACTOR"",
              ""ShortDescription"": ""Unsqrd. Super Factor"",
              ""LongDescription"": ""Unsquared Supercompresibility Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0003,
            ""Description"": ""1.0003""
          }
        ],
        ""Id"": ""fe426ad0-a293-434a-bbf9-e3b8e7381429""
      },
      ""TemperatureTest"": {
        ""VerificationTestId"": ""fe426ad0-a293-434a-bbf9-e3b8e7381429"",
        ""Gauge"": 90.0,
        ""PercentError"": 0.07,
        ""ActualFactor"": 0.9454,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""26\"":\""   89.61\"",\""35\"":\"" -0.1740\"",\""45\"":\""  0.9461\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   89.61"",
            ""Metadata"": {
              ""Number"": 26,
              ""Code"": ""GAS_TEMP"",
              ""ShortDescription"": ""Gas Temperat"",
              ""LongDescription"": ""EVC Gas Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 89.61,
            ""Description"": ""89.61""
          },
          {
            ""RawValue"": "" -0.1740"",
            ""Metadata"": {
              ""Number"": 35,
              ""Code"": """",
              ""ShortDescription"": ""Temperature"",
              ""LongDescription"": ""EVC Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": -0.1740,
            ""Description"": ""-0.1740""
          },
          {
            ""RawValue"": ""  0.9461"",
            ""Metadata"": {
              ""Number"": 45,
              ""Code"": ""TEMP_FACTOR"",
              ""ShortDescription"": ""Temperature Factor"",
              ""LongDescription"": ""Temperature Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.9461,
            ""Description"": ""0.9461""
          }
        ],
        ""Id"": ""fe426ad0-a293-434a-bbf9-e3b8e7381429""
      },
      ""VolumeTest"": null,
      ""SuperFactorTest"": {
        ""SuperFactorCaculations"": null,
        ""GaugeTemp"": 90.0,
        ""GaugePressure"": 20.00,
        ""EvcUnsqrFactor"": 1.0003,
        ""ActualFactor"": 1.0011,
        ""SuperFactorSquared"": 1.00220121,
        ""PercentError"": -0.08,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""53\"":\""  0.5778\"",\""54\"":\""  1.4260\"",\""55\"":\""  0.6970\"",\""110\"":\""       0\""}"",
        ""Items"": [
          {
            ""RawValue"": ""  0.5778"",
            ""Metadata"": {
              ""Number"": 53,
              ""Code"": ""SPEC_GR"",
              ""ShortDescription"": ""Specific Gravity"",
              ""LongDescription"": ""Specific Gravity"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.5778,
            ""Description"": ""0.5778""
          },
          {
            ""RawValue"": ""  1.4260"",
            ""Metadata"": {
              ""Number"": 54,
              ""Code"": ""N2"",
              ""ShortDescription"": ""% N2"",
              ""LongDescription"": ""% N2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.4260,
            ""Description"": ""1.4260""
          },
          {
            ""RawValue"": ""  0.6970"",
            ""Metadata"": {
              ""Number"": 55,
              ""Code"": ""CO2"",
              ""ShortDescription"": ""% CO2"",
              ""LongDescription"": ""% CO2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.6970,
            ""Description"": ""0.6970""
          },
          {
            ""RawValue"": ""       0"",
            ""Metadata"": {
              ""Number"": 110,
              ""Code"": ""FIXED_SUPER_FACTOR"",
              ""ShortDescription"": ""F Super Factor"",
              ""LongDescription"": ""Fixed Super Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": [
                {
                  ""Id"": 0,
                  ""Description"": ""Live"",
                  ""Value"": 0.0
                },
                {
                  ""Id"": 1,
                  ""Description"": ""Fixed"",
                  ""Value"": 1.0
                }
              ]
            },
            ""NumericValue"": 0.0,
            ""Description"": ""Live""
          }
        ],
        ""Id"": ""7f3f7f1c-5e11-4360-b4d0-99eeaca4abfe""
      },
      ""HasPassed"": true,
      ""Id"": ""fe426ad0-a293-434a-bbf9-e3b8e7381429""
    }
  ],
  ""TestDateTime"": ""2017-02-14T13:01:05.797"",
  ""ArchivedDateTime"": null,
  ""Type"": 3,
  ""InstrumentType"": {
    ""AccessCode"": 3,
    ""Name"": ""Mini-AT"",
    ""Id"": 3,
    ""ItemFilePath"": ""MiniATItems.xml""
  },
  ""CertificateId"": null,
  ""ClientId"": null,
  ""EmployeeId"": null,
  ""JobId"": """",
  ""ExportedDateTime"": null,
  ""EventLogPassed"": true,
  ""CommPortsPassed"": true,
  ""SerialNumber"": 13032608,
  ""InventoryNumber"": ""02693710"",
  ""InstrumentTypeString"": ""Prover.CommProtocol.Common.InstrumentType"",
  ""CompositionType"": 2,
  ""HasPassed"": true,
  ""FirmwareVersion"": 6.9200,
  ""PulseAScaling"": 2.0000,
  ""PulseASelect"": ""CorVol"",
  ""PulseBScaling"": 2.0000,
  ""PulseBSelect"": ""UncVol"",
  ""SiteNumber1"": 13032608.0,
  ""SiteNumber2"": 2693710.0,
  ""VolumeTest"": {
    ""PulseACount"": 5,
    ""PulseBCount"": 10,
    ""AppliedInput"": 100.00,
    ""DriveType"": {
      ""Energy"": {
        ""HasPassed"": true,
        ""PercentError"": -0.23,
        ""EvcEnergy"": 58.0,
        ""EnergyUnits"": ""Therms"",
        ""ActualEnergy"": 58.136
      },
      ""Discriminator"": ""Mechanical"",
      ""HasPassed"": true
    },
    ""TestInstrumentData"": ""{\""0\"":\""00000187\"",\""2\"":\""00000328\"",\""113\"":\""187.2885\"",\""140\"":\""00033347\""}"",
    ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
    ""VerificationTest"": {
      ""TestNumber"": 0,
      ""InstrumentId"": ""8d1ee376-f0a3-4119-b649-aeb20e2add49"",
      ""PressureTest"": {
        ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
        ""GasPressure"": 80.00,
        ""GasGauge"": 80.00,
        ""AtmosphericGauge"": 0.00,
        ""PercentError"": 0.27,
        ""ActualFactor"": 5.4311,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""8\"":\""   80.22\"",\""44\"":\""  5.4457\"",\""47\"":\""  1.0056\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   80.22"",
            ""Metadata"": {
              ""Number"": 8,
              ""Code"": ""GAS_PRESS"",
              ""ShortDescription"": ""Gas Press"",
              ""LongDescription"": ""Gas Pressure"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 80.22,
            ""Description"": ""80.22""
          },
          {
            ""RawValue"": ""  5.4457"",
            ""Metadata"": {
              ""Number"": 44,
              ""Code"": ""PRESS_FACTOR"",
              ""ShortDescription"": ""Press Factor"",
              ""LongDescription"": ""Pressure Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 5.4457,
            ""Description"": ""5.4457""
          },
          {
            ""RawValue"": ""  1.0056"",
            ""Metadata"": {
              ""Number"": 47,
              ""Code"": ""UNSQRD_SUPER_FACTOR"",
              ""ShortDescription"": ""Unsqrd. Super Factor"",
              ""LongDescription"": ""Unsquared Supercompresibility Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": true,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0056,
            ""Description"": ""1.0056""
          }
        ],
        ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
      },
      ""TemperatureTest"": {
        ""VerificationTestId"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7"",
        ""Gauge"": 32.0,
        ""PercentError"": -0.02,
        ""ActualFactor"": 1.0569,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""26\"":\""   32.11\"",\""35\"":\"" -0.1740\"",\""45\"":\""  1.0567\""}"",
        ""Items"": [
          {
            ""RawValue"": ""   32.11"",
            ""Metadata"": {
              ""Number"": 26,
              ""Code"": ""GAS_TEMP"",
              ""ShortDescription"": ""Gas Temperat"",
              ""LongDescription"": ""EVC Gas Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 32.11,
            ""Description"": ""32.11""
          },
          {
            ""RawValue"": "" -0.1740"",
            ""Metadata"": {
              ""Number"": 35,
              ""Code"": """",
              ""ShortDescription"": ""Temperature"",
              ""LongDescription"": ""EVC Temperature"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": -0.1740,
            ""Description"": ""-0.1740""
          },
          {
            ""RawValue"": ""  1.0567"",
            ""Metadata"": {
              ""Number"": 45,
              ""Code"": ""TEMP_FACTOR"",
              ""ShortDescription"": ""Temperature Factor"",
              ""LongDescription"": ""Temperature Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": true,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": false,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.0567,
            ""Description"": ""1.0567""
          }
        ],
        ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
      },
      ""SuperFactorTest"": {
        ""SuperFactorCaculations"": null,
        ""GaugeTemp"": 32.0,
        ""GaugePressure"": 80.00,
        ""EvcUnsqrFactor"": 1.0056,
        ""ActualFactor"": 1.0068,
        ""SuperFactorSquared"": 1.01364624,
        ""PercentError"": -0.12,
        ""InstrumentType"": {
          ""AccessCode"": 3,
          ""Name"": ""Mini-AT"",
          ""Id"": 3,
          ""ItemFilePath"": ""MiniATItems.xml""
        },
        ""HasPassed"": true,
        ""InstrumentData"": ""{\""53\"":\""  0.5778\"",\""54\"":\""  1.4260\"",\""55\"":\""  0.6970\"",\""110\"":\""       0\""}"",
        ""Items"": [
          {
            ""RawValue"": ""  0.5778"",
            ""Metadata"": {
              ""Number"": 53,
              ""Code"": ""SPEC_GR"",
              ""ShortDescription"": ""Specific Gravity"",
              ""LongDescription"": ""Specific Gravity"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.5778,
            ""Description"": ""0.5778""
          },
          {
            ""RawValue"": ""  1.4260"",
            ""Metadata"": {
              ""Number"": 54,
              ""Code"": ""N2"",
              ""ShortDescription"": ""% N2"",
              ""LongDescription"": ""% N2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 1.4260,
            ""Description"": ""1.4260""
          },
          {
            ""RawValue"": ""  0.6970"",
            ""Metadata"": {
              ""Number"": 55,
              ""Code"": ""CO2"",
              ""ShortDescription"": ""% CO2"",
              ""LongDescription"": ""% CO2"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": false,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": []
            },
            ""NumericValue"": 0.6970,
            ""Description"": ""0.6970""
          },
          {
            ""RawValue"": ""       0"",
            ""Metadata"": {
              ""Number"": 110,
              ""Code"": ""FIXED_SUPER_FACTOR"",
              ""ShortDescription"": ""F Super Factor"",
              ""LongDescription"": ""Fixed Super Factor"",
              ""IsAlarm"": false,
              ""IsPressure"": false,
              ""IsPressureTest"": false,
              ""IsTemperature"": false,
              ""IsTemperatureTest"": false,
              ""IsVolume"": true,
              ""IsVolumeTest"": false,
              ""IsSuperFactor"": true,
              ""ItemDescriptions"": [
                {
                  ""Id"": 0,
                  ""Description"": ""Live"",
                  ""Value"": 0.0
                },
                {
                  ""Id"": 1,
                  ""Description"": ""Fixed"",
                  ""Value"": 1.0
                }
              ]
            },
            ""NumericValue"": 0.0,
            ""Description"": ""Live""
          }
        ],
        ""Id"": ""3bb137bc-2443-49c3-92e9-3a7fbaf6ab4b""
      },
      ""HasPassed"": true,
      ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
    },
    ""AfterTestItems"": [
      {
        ""RawValue"": ""00000187"",
        ""Metadata"": {
          ""Number"": 0,
          ""Code"": ""COR_VOL"",
          ""ShortDescription"": ""Corrected Vol"",
          ""LongDescription"": ""Corrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 187.0,
        ""Description"": ""187""
      },
      {
        ""RawValue"": ""00000328"",
        ""Metadata"": {
          ""Number"": 2,
          ""Code"": ""UNCOR_VOL"",
          ""ShortDescription"": ""UnCor Vol"",
          ""LongDescription"": ""Uncorrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 328.0,
        ""Description"": ""328""
      },
      {
        ""RawValue"": ""187.2885"",
        ""Metadata"": {
          ""Number"": 113,
          ""Code"": ""HIGH_RES_CORRECTED"",
          ""ShortDescription"": ""High Res COR"",
          ""LongDescription"": ""High Resolution Corrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 187.2885,
        ""Description"": ""187.2885""
      },
      {
        ""RawValue"": ""00033347"",
        ""Metadata"": {
          ""Number"": 140,
          ""Code"": ""ENERGY"",
          ""ShortDescription"": ""Energy"",
          ""LongDescription"": ""Energy"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 33347.0,
        ""Description"": ""33347""
      }
    ],
    ""UnCorrectedPercentError"": 0.0,
    ""TrueUncorrected"": 1000.00,
    ""CorrectedPercentError"": -0.08,
    ""CorrectedHasPassed"": true,
    ""UnCorrectedHasPassed"": true,
    ""HasPassed"": true,
    ""PercentError"": null,
    ""ActualFactor"": null,
    ""UncPulseCount"": 10,
    ""CorPulseCount"": 5,
    ""UnCorPulsesPassed"": true,
    ""CorPulsesPassed"": true,
    ""TrueCorrected"": 5818.460776016241600000,
    ""EvcCorrected"": 5813.6000,
    ""EvcUncorrected"": 1000.0,
    ""DriveTypeDiscriminator"": ""Mechanical"",
    ""InstrumentType"": {
      ""AccessCode"": 3,
      ""Name"": ""Mini-AT"",
      ""Id"": 3,
      ""ItemFilePath"": ""MiniATItems.xml""
    },
    ""InstrumentData"": ""{\""0\"":\""00000181\"",\""2\"":\""00000318\"",\""113\"":\""181.4749\"",\""140\"":\""00033289\""}"",
    ""Items"": [
      {
        ""RawValue"": ""00000181"",
        ""Metadata"": {
          ""Number"": 0,
          ""Code"": ""COR_VOL"",
          ""ShortDescription"": ""Corrected Vol"",
          ""LongDescription"": ""Corrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 181.0,
        ""Description"": ""181""
      },
      {
        ""RawValue"": ""00000318"",
        ""Metadata"": {
          ""Number"": 2,
          ""Code"": ""UNCOR_VOL"",
          ""ShortDescription"": ""UnCor Vol"",
          ""LongDescription"": ""Uncorrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 318.0,
        ""Description"": ""318""
      },
      {
        ""RawValue"": ""181.4749"",
        ""Metadata"": {
          ""Number"": 113,
          ""Code"": ""HIGH_RES_CORRECTED"",
          ""ShortDescription"": ""High Res COR"",
          ""LongDescription"": ""High Resolution Corrected Volume"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 181.4749,
        ""Description"": ""181.4749""
      },
      {
        ""RawValue"": ""00033289"",
        ""Metadata"": {
          ""Number"": 140,
          ""Code"": ""ENERGY"",
          ""ShortDescription"": ""Energy"",
          ""LongDescription"": ""Energy"",
          ""IsAlarm"": false,
          ""IsPressure"": false,
          ""IsPressureTest"": false,
          ""IsTemperature"": false,
          ""IsTemperatureTest"": false,
          ""IsVolume"": true,
          ""IsVolumeTest"": true,
          ""IsSuperFactor"": false,
          ""ItemDescriptions"": []
        },
        ""NumericValue"": 33289.0,
        ""Description"": ""33289""
      }
    ],
    ""Id"": ""e0d3fc9c-2ac2-4bc9-ba06-c04a97bf7af7""
  },
  ""InstrumentData"": ""{\""0\"":\""00000181\"",\""2\"":\""00000317\"",\""5\"":\""00000000\"",\""6\"":\""00000000\"",\""8\"":\""   14.16\"",\""10\"":\""  100.00\"",\""11\"":\""   -1.00\"",\""12\"":\""  0.0000\"",\""13\"":\"" 14.7300\"",\""14\"":\"" 14.7000\"",\""26\"":\""   32.41\"",\""27\"":\""  -30.00\"",\""28\"":\""  120.00\"",\""34\"":\""   60.00\"",\""35\"":\"" -0.1740\"",\""44\"":\""  0.9614\"",\""45\"":\""  1.0561\"",\""47\"":\""  1.0000\"",\""53\"":\""  0.5778\"",\""54\"":\""  1.4260\"",\""55\"":\""  0.6970\"",\""56\"":\""  2.0000\"",\""57\"":\""  2.0000\"",\""62\"":\""13032608\"",\""87\"":\""       1\"",\""89\"":\""       0\"",\""90\"":\""       6\"",\""92\"":\""       5\"",\""93\"":\""       0\"",\""94\"":\""       2\"",\""98\"":\""       2\"",\""109\"":\""       0\"",\""110\"":\""       0\"",\""111\"":\""       0\"",\""112\"":\""       1\"",\""113\"":\""181.0092\"",\""122\"":\""  6.9200\"",\""137\"":\""  100.00\"",\""140\"":\""00033284\"",\""141\"":\""       0\"",\""142\"":\"" 1000.00\"",\""147\"":\""       0\"",\""200\"":\""13032608\"",\""201\"":\""02693710\""}"",
  ""Items"": [
    {
      ""RawValue"": ""00000181"",
      ""Metadata"": {
        ""Number"": 0,
        ""Code"": ""COR_VOL"",
        ""ShortDescription"": ""Corrected Vol"",
        ""LongDescription"": ""Corrected Volume"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": true,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 181.0,
      ""Description"": ""181""
    },
    {
      ""RawValue"": ""00000317"",
      ""Metadata"": {
        ""Number"": 2,
        ""Code"": ""UNCOR_VOL"",
        ""ShortDescription"": ""UnCor Vol"",
        ""LongDescription"": ""Uncorrected Volume"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": true,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 317.0,
      ""Description"": ""317""
    },
    {
      ""RawValue"": ""00000000"",
      ""Metadata"": {
        ""Number"": 5,
        ""Code"": ""PULSER_A"",
        ""ShortDescription"": ""Pulser A #"",
        ""LongDescription"": ""Pulser A # o"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.0,
      ""Description"": ""0""
    },
    {
      ""RawValue"": ""00000000"",
      ""Metadata"": {
        ""Number"": 6,
        ""Code"": ""PULSER_B"",
        ""ShortDescription"": ""Pulser B #"",
        ""LongDescription"": ""Pulser B # o"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.0,
      ""Description"": ""0""
    },
    {
      ""RawValue"": ""   14.16"",
      ""Metadata"": {
        ""Number"": 8,
        ""Code"": ""GAS_PRESS"",
        ""ShortDescription"": ""Gas Press"",
        ""LongDescription"": ""Gas Pressure"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": true,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 14.16,
      ""Description"": ""14.16""
    },
    {
      ""RawValue"": ""  100.00"",
      ""Metadata"": {
        ""Number"": 10,
        ""Code"": """",
        ""ShortDescription"": ""Press High A"",
        ""LongDescription"": ""Pressure High Alarm"",
        ""IsAlarm"": true,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 100.00,
      ""Description"": ""100.00""
    },
    {
      ""RawValue"": ""   -1.00"",
      ""Metadata"": {
        ""Number"": 11,
        ""Code"": """",
        ""ShortDescription"": ""Press Low A"",
        ""LongDescription"": ""Pressure Low Alarm"",
        ""IsAlarm"": true,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": -1.00,
      ""Description"": ""-1.00""
    },
    {
      ""RawValue"": ""  0.0000"",
      ""Metadata"": {
        ""Number"": 12,
        ""Code"": """",
        ""ShortDescription"": ""P Cal Atmos"",
        ""LongDescription"": ""Pressure Corrected Atmospheric"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.0000,
      ""Description"": ""0.0000""
    },
    {
      ""RawValue"": "" 14.7300"",
      ""Metadata"": {
        ""Number"": 13,
        ""Code"": ""BASE_PRESS"",
        ""ShortDescription"": ""Base Press"",
        ""LongDescription"": ""Base Pressure"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 14.7300,
      ""Description"": ""14.7300""
    },
    {
      ""RawValue"": "" 14.7000"",
      ""Metadata"": {
        ""Number"": 14,
        ""Code"": ""ATM_PRESS"",
        ""ShortDescription"": ""ATM Press"",
        ""LongDescription"": ""ATM Pressure"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 14.7000,
      ""Description"": ""14.7000""
    },
    {
      ""RawValue"": ""   32.41"",
      ""Metadata"": {
        ""Number"": 26,
        ""Code"": ""GAS_TEMP"",
        ""ShortDescription"": ""Gas Temperat"",
        ""LongDescription"": ""EVC Gas Temperature"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": true,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 32.41,
      ""Description"": ""32.41""
    },
    {
      ""RawValue"": ""  -30.00"",
      ""Metadata"": {
        ""Number"": 27,
        ""Code"": """",
        ""ShortDescription"": ""Gas Temp Lo"",
        ""LongDescription"": ""Gas Temp Lo"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": true,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": -30.00,
      ""Description"": ""-30.00""
    },
    {
      ""RawValue"": ""  120.00"",
      ""Metadata"": {
        ""Number"": 28,
        ""Code"": """",
        ""ShortDescription"": ""Gas Temp Hi"",
        ""LongDescription"": ""Gas Temp Hi"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": true,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 120.00,
      ""Description"": ""120.00""
    },
    {
      ""RawValue"": ""   60.00"",
      ""Metadata"": {
        ""Number"": 34,
        ""Code"": """",
        ""ShortDescription"": ""Base Temp"",
        ""LongDescription"": ""Base Temperature"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": true,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 60.00,
      ""Description"": ""60.00""
    },
    {
      ""RawValue"": "" -0.1740"",
      ""Metadata"": {
        ""Number"": 35,
        ""Code"": """",
        ""ShortDescription"": ""Temperature"",
        ""LongDescription"": ""EVC Temperature"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": true,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": -0.1740,
      ""Description"": ""-0.1740""
    },
    {
      ""RawValue"": ""  0.9614"",
      ""Metadata"": {
        ""Number"": 44,
        ""Code"": ""PRESS_FACTOR"",
        ""ShortDescription"": ""Press Factor"",
        ""LongDescription"": ""Pressure Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": true,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.9614,
      ""Description"": ""0.9614""
    },
    {
      ""RawValue"": ""  1.0561"",
      ""Metadata"": {
        ""Number"": 45,
        ""Code"": ""TEMP_FACTOR"",
        ""ShortDescription"": ""Temperature Factor"",
        ""LongDescription"": ""Temperature Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": true,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 1.0561,
      ""Description"": ""1.0561""
    },
    {
      ""RawValue"": ""  1.0000"",
      ""Metadata"": {
        ""Number"": 47,
        ""Code"": ""UNSQRD_SUPER_FACTOR"",
        ""ShortDescription"": ""Unsqrd. Super Factor"",
        ""LongDescription"": ""Unsquared Supercompresibility Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": true,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 1.0000,
      ""Description"": ""1.0000""
    },
    {
      ""RawValue"": ""  0.5778"",
      ""Metadata"": {
        ""Number"": 53,
        ""Code"": ""SPEC_GR"",
        ""ShortDescription"": ""Specific Gravity"",
        ""LongDescription"": ""Specific Gravity"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": true,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.5778,
      ""Description"": ""0.5778""
    },
    {
      ""RawValue"": ""  1.4260"",
      ""Metadata"": {
        ""Number"": 54,
        ""Code"": ""N2"",
        ""ShortDescription"": ""% N2"",
        ""LongDescription"": ""% N2"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": true,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 1.4260,
      ""Description"": ""1.4260""
    },
    {
      ""RawValue"": ""  0.6970"",
      ""Metadata"": {
        ""Number"": 55,
        ""Code"": ""CO2"",
        ""ShortDescription"": ""% CO2"",
        ""LongDescription"": ""% CO2"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": true,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.6970,
      ""Description"": ""0.6970""
    },
    {
      ""RawValue"": ""  2.0000"",
      ""Metadata"": {
        ""Number"": 56,
        ""Code"": ""PULSER_A_SCALING"",
        ""ShortDescription"": ""Pulser A Scaling"",
        ""LongDescription"": ""Pulser A Scaling"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 2.0000,
      ""Description"": ""2.0000""
    },
    {
      ""RawValue"": ""  2.0000"",
      ""Metadata"": {
        ""Number"": 57,
        ""Code"": ""PULSER_B_SCALING"",
        ""ShortDescription"": ""Pulser B Scaling"",
        ""LongDescription"": ""Pulser B Scaling"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 2.0000,
      ""Description"": ""2.0000""
    },
    {
      ""RawValue"": ""13032608"",
      ""Metadata"": {
        ""Number"": 62,
        ""Code"": ""SERIAL_NUM"",
        ""ShortDescription"": ""Serial Number"",
        ""LongDescription"": ""Serial Number"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 13032608.0,
      ""Description"": ""13032608""
    },
    {
      ""RawValue"": ""       1"",
      ""Metadata"": {
        ""Number"": 87,
        ""Code"": ""PRESS_UNITS"",
        ""ShortDescription"": ""Press Units"",
        ""LongDescription"": ""Pressure Units"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""PSIG"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""PSIA"",
            ""Value"": 1.0
          },
          {
            ""Id"": 2,
            ""Description"": ""kPa"",
            ""Value"": 2.0
          },
          {
            ""Id"": 3,
            ""Description"": ""mPa"",
            ""Value"": 3.0
          },
          {
            ""Id"": 4,
            ""Description"": ""BAR"",
            ""Value"": 4.0
          },
          {
            ""Id"": 5,
            ""Description"": ""mBAR"",
            ""Value"": 5.0
          },
          {
            ""Id"": 6,
            ""Description"": ""KGcm2"",
            ""Value"": 6.0
          },
          {
            ""Id"": 7,
            ""Description"": ""inWC"",
            ""Value"": 7.0
          },
          {
            ""Id"": 8,
            ""Description"": ""inHG"",
            ""Value"": 8.0
          },
          {
            ""Id"": 9,
            ""Description"": ""mmHG"",
            ""Value"": 9.0
          }
        ]
      },
      ""NumericValue"": 1.0,
      ""Description"": ""PSIA""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 89,
        ""Code"": ""TEMP_UNITS"",
        ""ShortDescription"": ""Temperature Units"",
        ""LongDescription"": ""Temperature Units"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": true,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""F"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""C"",
            ""Value"": 1.0
          },
          {
            ""Id"": 2,
            ""Description"": ""R"",
            ""Value"": 2.0
          },
          {
            ""Id"": 3,
            ""Description"": ""K"",
            ""Value"": 3.0
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""F""
    },
    {
      ""RawValue"": ""       6"",
      ""Metadata"": {
        ""Number"": 90,
        ""Code"": ""Corr Vol Units"",
        ""ShortDescription"": ""Corr Vol Units"",
        ""LongDescription"": ""Corrected Volume Units"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""CuFT"",
            ""Value"": 1.0
          },
          {
            ""Id"": 1,
            ""Description"": ""CuFTx10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 2,
            ""Description"": ""CuFTx100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 3,
            ""Description"": ""CF"",
            ""Value"": 1.0
          },
          {
            ""Id"": 4,
            ""Description"": ""CFx10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 5,
            ""Description"": ""CFx100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 6,
            ""Description"": ""CFx1000"",
            ""Value"": 1000.0
          },
          {
            ""Id"": 7,
            ""Description"": ""CCF"",
            ""Value"": 100.0
          },
          {
            ""Id"": 8,
            ""Description"": ""MCF"",
            ""Value"": 1000.0
          }
        ]
      },
      ""NumericValue"": 1000.0,
      ""Description"": ""CFx1000""
    },
    {
      ""RawValue"": ""       5"",
      ""Metadata"": {
        ""Number"": 92,
        ""Code"": ""Uncorr Volume Units"",
        ""ShortDescription"": ""Uncorr Volume Units"",
        ""LongDescription"": ""UnCorrected Volume Units"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""CuFT"",
            ""Value"": 1.0
          },
          {
            ""Id"": 1,
            ""Description"": ""CuFTx10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 2,
            ""Description"": ""CuFTx100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 3,
            ""Description"": ""CF"",
            ""Value"": 1.0
          },
          {
            ""Id"": 4,
            ""Description"": ""CFx10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 5,
            ""Description"": ""CFx100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 6,
            ""Description"": ""CFx1000"",
            ""Value"": 1000.0
          },
          {
            ""Id"": 7,
            ""Description"": ""CCF"",
            ""Value"": 100.0
          },
          {
            ""Id"": 8,
            ""Description"": ""MCF"",
            ""Value"": 1000.0
          }
        ]
      },
      ""NumericValue"": 100.0,
      ""Description"": ""CFx100""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 93,
        ""Code"": ""PULSER_A"",
        ""ShortDescription"": ""Pulser A"",
        ""LongDescription"": ""Pulser A"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""CorVol"",
            ""Value"": null
          },
          {
            ""Id"": 1,
            ""Description"": ""PCorVol"",
            ""Value"": null
          },
          {
            ""Id"": 2,
            ""Description"": ""UncVol"",
            ""Value"": null
          },
          {
            ""Id"": 3,
            ""Description"": ""NoOut"",
            ""Value"": null
          },
          {
            ""Id"": 4,
            ""Description"": ""Time"",
            ""Value"": null
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""CorVol""
    },
    {
      ""RawValue"": ""       2"",
      ""Metadata"": {
        ""Number"": 94,
        ""Code"": ""PULSER_B"",
        ""ShortDescription"": ""Pulser B"",
        ""LongDescription"": ""Pulser B"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""CorVol"",
            ""Value"": null
          },
          {
            ""Id"": 1,
            ""Description"": ""PCorVol"",
            ""Value"": null
          },
          {
            ""Id"": 2,
            ""Description"": ""UncVol"",
            ""Value"": null
          },
          {
            ""Id"": 3,
            ""Description"": ""NoOut"",
            ""Value"": null
          },
          {
            ""Id"": 4,
            ""Description"": ""Time"",
            ""Value"": null
          }
        ]
      },
      ""NumericValue"": 2.0,
      ""Description"": ""UncVol""
    },
    {
      ""RawValue"": ""       2"",
      ""Metadata"": {
        ""Number"": 98,
        ""Code"": ""METER_INDEX"",
        ""ShortDescription"": ""Meter Index Rate"",
        ""LongDescription"": ""Meter Index Rate"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""CF1"",
            ""Value"": 1.0
          },
          {
            ""Id"": 1,
            ""Description"": ""CF5"",
            ""Value"": 5.0
          },
          {
            ""Id"": 2,
            ""Description"": ""CF10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 3,
            ""Description"": ""CF100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 4,
            ""Description"": ""CF1000"",
            ""Value"": 1000.0
          },
          {
            ""Id"": 5,
            ""Description"": ""m3x.1"",
            ""Value"": 0.1
          },
          {
            ""Id"": 6,
            ""Description"": ""m3x1"",
            ""Value"": 1.0
          },
          {
            ""Id"": 7,
            ""Description"": ""m3x10"",
            ""Value"": 10.0
          },
          {
            ""Id"": 8,
            ""Description"": ""m3x100"",
            ""Value"": 100.0
          },
          {
            ""Id"": 9,
            ""Description"": ""m3x1000"",
            ""Value"": 1000.0
          },
          {
            ""Id"": 10,
            ""Description"": ""CF10000"",
            ""Value"": 10000.0
          },
          {
            ""Id"": 11,
            ""Description"": ""CF0"",
            ""Value"": 0.0
          },
          {
            ""Id"": 12,
            ""Description"": ""CF50"",
            ""Value"": 50.0
          },
          {
            ""Id"": 13,
            ""Description"": ""CF500"",
            ""Value"": 500.0
          },
          {
            ""Id"": 14,
            ""Description"": ""Rotary"",
            ""Value"": 14.0
          }
        ]
      },
      ""NumericValue"": 10.0,
      ""Description"": ""CF10""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 109,
        ""Code"": ""FIXED_PRESS_FACTOR"",
        ""ShortDescription"": ""F Press Factor"",
        ""LongDescription"": ""Fixed Pressure Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""Live"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""Fixed"",
            ""Value"": 1.0
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""Live""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 110,
        ""Code"": ""FIXED_SUPER_FACTOR"",
        ""ShortDescription"": ""F Super Factor"",
        ""LongDescription"": ""Fixed Super Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": true,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""Live"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""Fixed"",
            ""Value"": 1.0
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""Live""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 111,
        ""Code"": ""FIXED_TEMP_FACTOR"",
        ""ShortDescription"": ""F Temp Factor"",
        ""LongDescription"": ""Fixed Temp Factor"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": true,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""Live"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""Fixed"",
            ""Value"": 1.0
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""Live""
    },
    {
      ""RawValue"": ""       1"",
      ""Metadata"": {
        ""Number"": 112,
        ""Code"": ""TRANSDUCER_TYPE"",
        ""ShortDescription"": ""Transducer Type"",
        ""LongDescription"": ""Transducer Type"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""Gauge"",
            ""Value"": 0.0
          },
          {
            ""Id"": 1,
            ""Description"": ""Absolute"",
            ""Value"": 1.0
          }
        ]
      },
      ""NumericValue"": 1.0,
      ""Description"": ""Absolute""
    },
    {
      ""RawValue"": ""181.0092"",
      ""Metadata"": {
        ""Number"": 113,
        ""Code"": ""HIGH_RES_CORRECTED"",
        ""ShortDescription"": ""High Res COR"",
        ""LongDescription"": ""High Resolution Corrected Volume"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": true,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 181.0092,
      ""Description"": ""181.0092""
    },
    {
      ""RawValue"": ""  6.9200"",
      ""Metadata"": {
        ""Number"": 122,
        ""Code"": ""FIRMwARE"",
        ""ShortDescription"": ""Firm. Ver."",
        ""LongDescription"": ""Firmware Version"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 6.9200,
      ""Description"": ""6.9200""
    },
    {
      ""RawValue"": ""  100.00"",
      ""Metadata"": {
        ""Number"": 137,
        ""Code"": ""PRESS_RANGE"",
        ""ShortDescription"": ""Pressure Range"",
        ""LongDescription"": ""Pressure Range"",
        ""IsAlarm"": false,
        ""IsPressure"": true,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 100.00,
      ""Description"": ""100.00""
    },
    {
      ""RawValue"": ""00033284"",
      ""Metadata"": {
        ""Number"": 140,
        ""Code"": ""ENERGY"",
        ""ShortDescription"": ""Energy"",
        ""LongDescription"": ""Energy"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": true,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 33284.0,
      ""Description"": ""33284""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 141,
        ""Code"": ""ENERGY_UNITS"",
        ""ShortDescription"": ""Energy Units"",
        ""LongDescription"": ""Energy Units"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": [
          {
            ""Id"": 0,
            ""Description"": ""Therms"",
            ""Value"": null
          },
          {
            ""Id"": 1,
            ""Description"": ""DecaTherms"",
            ""Value"": null
          },
          {
            ""Id"": 2,
            ""Description"": ""MegaJoules"",
            ""Value"": null
          },
          {
            ""Id"": 3,
            ""Description"": ""GigaJoules"",
            ""Value"": null
          },
          {
            ""Id"": 4,
            ""Description"": ""KiloCals"",
            ""Value"": null
          },
          {
            ""Id"": 5,
            ""Description"": ""KiloWattHours"",
            ""Value"": null
          }
        ]
      },
      ""NumericValue"": 0.0,
      ""Description"": ""Therms""
    },
    {
      ""RawValue"": "" 1000.00"",
      ""Metadata"": {
        ""Number"": 142,
        ""Code"": ""GAS_ENERGY_VALUE"",
        ""ShortDescription"": ""Gas Energy Value"",
        ""LongDescription"": ""Gas Energy Value"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 1000.00,
      ""Description"": ""1000.00""
    },
    {
      ""RawValue"": ""       0"",
      ""Metadata"": {
        ""Number"": 147,
        ""Code"": ""SUPER_TABLE"",
        ""ShortDescription"": ""Super Compress. Table"",
        ""LongDescription"": ""Super Compress. Table"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": true,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 0.0,
      ""Description"": ""0""
    },
    {
      ""RawValue"": ""13032608"",
      ""Metadata"": {
        ""Number"": 200,
        ""Code"": ""SITENUMBER1"",
        ""ShortDescription"": ""Site Number 1"",
        ""LongDescription"": ""Site Number 1"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 13032608.0,
      ""Description"": ""13032608""
    },
    {
      ""RawValue"": ""02693710"",
      ""Metadata"": {
        ""Number"": 201,
        ""Code"": ""SITENUMBER2"",
        ""ShortDescription"": ""Site Number 2"",
        ""LongDescription"": ""Site Number 2"",
        ""IsAlarm"": false,
        ""IsPressure"": false,
        ""IsPressureTest"": false,
        ""IsTemperature"": false,
        ""IsTemperatureTest"": false,
        ""IsVolume"": false,
        ""IsVolumeTest"": false,
        ""IsSuperFactor"": false,
        ""ItemDescriptions"": []
      },
      ""NumericValue"": 2693710.0,
      ""Description"": ""2693710""
    }
  ],
  ""Id"": ""8d1ee376-f0a3-4119-b649-aeb20e2add49""
}";

        #endregion
    }
}
