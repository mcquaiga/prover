//using System;
//using System.Globalization;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using NLog;
//using PressureTest = UnionGas.MASA.DCRWebService.PressureTest;
//using SuperFactorTest = UnionGas.MASA.DCRWebService.SuperFactorTest;
//using TemperatureTest = UnionGas.MASA.DCRWebService.TemperatureTest;
//using VerificationTest = UnionGas.MASA.DCRWebService.VerificationTest;
//using VolumeTest = UnionGas.MASA.DCRWebService.VolumeTest;

//namespace Modules.UnionGas.Exporter
//{
//    public static class Translate
//    {
//        private const int ProverNumberId = 239;
//        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

//        public static QARunEvcTestResult RunTranslationForExport(Instrument instrument)
//        {
//            var qaRun = new QARunEvcTestResult
//            {
//                InstrumentType = instrument.InstrumentType.Name,
//                InventoryCode = instrument.SiteNumber2.ToString(CultureInfo.InvariantCulture).PadLeft(7, '0'),
//                TestDate = instrument.TestDateTime,
//                DriveType = instrument.VolumeTest.DriveTypeDiscriminator,
//                MeterType = instrument.VolumeTest.DriveTypeDiscriminator == "Rotary" ? (instrument.VolumeTest.DriveType as RotaryDrive).Meter.MeterTypeDescription : string.Empty,
//                MeterDisplacement = instrument.VolumeTest.DriveTypeDiscriminator == "Rotary" ? 
//                    Convert.ToDouble((instrument.VolumeTest.DriveType as RotaryDrive).Meter.MeterDisplacement) : 0,
//                ConfirmedStatus = instrument.HasPassed ? "PASS" : "FAIL",
//                FirmwareVersion = Convert.ToDouble(instrument.FirmwareVersion),
//                SerialNumber = instrument.SerialNumber.ToString(),
//                InstrumentData = instrument.InstrumentData,
//                InstrumentComposition = instrument.CompositionType.ToString(),
//                EmployeeId = instrument.EmployeeId,
//                EventLogPassedInd = instrument.EventLogPassed.HasValue ? instrument.EventLogPassed.Value ? "Y" : "N" : null,
//                CommPortPassedInd = instrument.CommPortsPassed.HasValue ? instrument.CommPortsPassed.Value ? "Y" : "N" : null,
//                PressureInfo = new PressureHeader
//                {
//                    BasePressure =
//                        instrument.EvcBasePressure().HasValue
//                            ? Convert.ToDouble(decimal.Round(instrument.EvcBasePressure().Value, 2)) : 0,
//                    PressureRange = Convert.ToDouble(instrument.EvcPressureRange()),
//                    PressureUnits = instrument.PressureUnits(),
//                    TransducerType = instrument.GetTransducerType().ToString().Substring(0, 1),
//                    ProgrammedAtmosphericPressure =
//                        instrument.EvcAtmosphericPressure().HasValue ? Convert.ToDouble(decimal.Round(instrument.EvcAtmosphericPressure().Value, 2)) : 0
//                },
//                TemperatureInfo = new TemperatureHeader
//                {
//                    BaseTemperature =
//                        instrument.EvcBaseTemperature().HasValue ? Convert.ToDouble(decimal.Round(instrument.EvcBaseTemperature().Value, 2)) : 0,
//                    TemperatureRange = "-40 to 170",
//                    TemperatureUnits = $"deg{instrument.TemperatureUnits()}"
//                },
//                SuperFactorInfo = new SuperFactorHeader
//                {
//                    CO2 = instrument.CO2().HasValue ? Convert.ToDouble(decimal.Round(instrument.CO2().Value, 4)) : 0,
//                    SpecGr = instrument.SpecGr().HasValue ? Convert.ToDouble(decimal.Round(instrument.SpecGr().Value, 4)) : 0,
//                    N2 = instrument.N2().HasValue ? Convert.ToDouble(decimal.Round(instrument.N2().Value, 4)) : 0,
//                    FPVTable = "NX19"
//                },
//                VolumeInfo = new VolumeHeader
//                {
//                    CorrectedMultiplierDescription = instrument.CorrectedMultiplierDescription(),
//                    CorrectedMultiplierValue =
//                        (int)instrument.CorrectedMultiplier(),
//                    UncorrectedMultiplierDescription = instrument.UnCorrectedMultiplierDescription(),
//                    UncorrectedMultiplierValue =
//                        (int)instrument.UnCorrectedMultiplier(),
//                    DriveRateDescription = instrument.DriveRateDescription(),
//                    PulseASelect = instrument.PulseASelect(),
//                    PulseBSelect = instrument.PulseBSelect()
//                },

//                VerificationTests = instrument.VerificationTests.Select(TranslateVerificationTest)
//                        .OrderBy(t => t.SequenceNumber)
//                        .ToArray(),

//                IndexReading = (int)instrument.Items.GetItem(98).NumericValue,
//                Comments = string.Empty,
//                JobNumber = !string.IsNullOrEmpty(instrument.JobId) ? int.Parse(instrument.JobId) : -1,
//                ProverNumber = ProverNumberId.ToString(), //
//                MeterClassCode = "EV",
//                TestReason = "6",
//                FieldMeterDesc = string.Empty,
//                SubmitRunIndicator = "Y"
//            };

//            Task.Run(() =>
//            {
//                var objectString = JsonConvert.SerializeObject(qaRun, Formatting.Indented,
//                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
//                Log.Debug($"Exporting Instrument object to MASA: {Environment.NewLine} {objectString}");
//            });

//            return qaRun;
//        }

//        public static QARunEvcTestResult CreateFailedTestForExport(MeterDTO meterDto, string employeeId)
//        {
//            Log.Info(JsonConvert.SerializeObject(meterDto));
//            try
//            {
//                var qaRun = new QARunEvcTestResult
//                {
//                    InstrumentType = meterDto.ModelName,
//                    InventoryCode = meterDto.InventoryCode.PadLeft(7, '0'),
//                    TestDate = DateTime.Now,
//                    DriveType = string.Empty,
//                    MeterType = string.Empty,
//                    MeterDisplacement = 0,
//                    ConfirmedStatus = "FAIL",
//                    FirmwareVersion = 0d,
//                    SerialNumber = meterDto.SerialNumber,
//                    InstrumentData = string.Empty,
//                    InstrumentComposition = string.Empty,
//                    EmployeeId = employeeId,
//                    EventLogPassedInd = "N",
//                    CommPortPassedInd = "N",
//                    PressureInfo = new PressureHeader
//                    {
//                        BasePressure = 0,
//                        PressureRange = 0,
//                        PressureUnits = string.Empty,
//                        TransducerType = string.Empty,
//                        ProgrammedAtmosphericPressure = 0
//                    },
//                    TemperatureInfo = new TemperatureHeader
//                    {
//                        BaseTemperature = 0,
//                        TemperatureRange = "-40 to 170",
//                        TemperatureUnits = $"degF"
//                    },
//                    SuperFactorInfo = new SuperFactorHeader
//                    {
//                        CO2 = 0,
//                        SpecGr = 0,
//                        N2 = 0,
//                        FPVTable = "NX19"
//                    },
//                    VolumeInfo = new VolumeHeader
//                    {
//                        CorrectedMultiplierDescription = string.Empty,
//                        CorrectedMultiplierValue = 0,
//                        UncorrectedMultiplierDescription = string.Empty,
//                        UncorrectedMultiplierValue = 0,
//                        DriveRateDescription = string.Empty,
//                        PulseASelect = string.Empty,
//                        PulseBSelect = string.Empty
//                    },

//                    VerificationTests = null,

//                    IndexReading = 0,
//                    Comments = string.Empty,
//                    JobNumber = meterDto.JobNumber.Value,
//                    ProverNumber = ProverNumberId.ToString(), //
//                    MeterClassCode = "EV",
//                    TestReason = "6",
//                    FieldMeterDesc = string.Empty,
//                    SubmitRunIndicator = "Y"
//                };
//                Log.Info("Failed Object:" + JsonConvert.SerializeObject(qaRun).ToString());
                
//                return qaRun;
//            }
//            catch(Exception ex)
//            {
//                Log.Error(ex);
//            }
            
//            return null;
//        }

//        private static VerificationTest TranslateVerificationTest(Prover.Core.Models.Instruments.VerificationTest vt)
//        {
//            return new VerificationTest
//            {
//                SequenceNumber = vt.TestNumber + 1,
//                CapacityLevelTypeCode = vt.TestNumber + 1 == 1 ? "L" : vt.TestNumber + 1 == 2 ? "M" : "H",
//                Pressure = TranslatePressureTest(vt),
//                Temperature = TranslateTemperatureTest(vt),
//                SuperFactor = TranslateSuperFactorTest(vt),
//                Volume = TranslateVolumeTest(vt)
//            };
//        }

//        private static VolumeTest TranslateVolumeTest(Prover.Core.Models.Instruments.VerificationTest vt)
//        {
//            if (vt.VolumeTest == null) return new VolumeTest();

//            var mechanicalDrive = vt.VolumeTest.DriveType as MechanicalDrive;

//            return new VolumeTest
//            {
//                AppliedInput = Convert.ToDouble(RoundTo(vt.VolumeTest.AppliedInput, 2)),
//                EvcCorrected = Convert.ToDouble(RoundTo(vt.VolumeTest.EvcCorrected, 2)),
//                EvcUncorrected = Convert.ToDouble(RoundTo(vt.VolumeTest.EvcUncorrected, 2)),
//                CorPulseCount = vt.VolumeTest.CorPulseCount,
//                UncPulseCount = vt.VolumeTest.UncPulseCount,
//                PulseACount = vt.VolumeTest.PulseACount,
//                PulseBCount = vt.VolumeTest.PulseBCount,
//                TrueCorrected = Convert.ToDouble(RoundTo(vt.VolumeTest.TrueCorrected, 4)),
//                CorrectedPercentError = Convert.ToDouble(RoundTo(vt.VolumeTest.CorrectedPercentError, 2)),
//                UnCorrectedPercentError = Convert.ToDouble(RoundTo(vt.VolumeTest.UnCorrectedPercentError, 2)),
//                EnergyPassedInd = mechanicalDrive != null ? mechanicalDrive.Energy.HasPassed ? "Y" : "N" : null
//            };
//        }

//        private static SuperFactorTest TranslateSuperFactorTest(Prover.Core.Models.Instruments.VerificationTest vt)
//        {
//            if (vt.SuperFactorTest == null) return new SuperFactorTest();

//            return new SuperFactorTest
//            {
//                ActualFactor = Convert.ToDouble(RoundTo(vt.SuperFactorTest.ActualFactor, 4)),
//                EvcFactor = Convert.ToDouble(RoundTo(vt.SuperFactorTest.EvcUnsqrFactor, 4)),
//                EvcUnsqrFactor = Convert.ToDouble(RoundTo(vt.SuperFactorTest.EvcUnsqrFactor, 4)),
//                GaugePressure = Convert.ToDouble(RoundTo(vt.SuperFactorTest.GaugePressure, 2)),
//                GaugeTemperature = Convert.ToDouble(RoundTo(vt.SuperFactorTest.GaugeTemp, 2)),
//                PercentError = Convert.ToDouble(RoundTo(vt.SuperFactorTest.PercentError, 2))
//            };
//        }

//        private static TemperatureTest TranslateTemperatureTest(Prover.Core.Models.Instruments.VerificationTest vt)
//        {
//            if (vt.TemperatureTest == null) return new TemperatureTest();

//            return new TemperatureTest
//            {
//                ActualFactor = Convert.ToDouble(RoundTo(vt.TemperatureTest.ActualFactor, 4)),
//                GaugeTemperature = Convert.ToDouble(RoundTo((decimal) vt.TemperatureTest.Gauge, 2)),
//                EvcFactor = Convert.ToDouble(RoundTo(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.Factor).NumericValue, 4)),
//                EvcTemperature = Convert.ToDouble(RoundTo(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.GasTemperature).NumericValue, 2)),
//                PercentError = Convert.ToDouble(RoundTo(vt.TemperatureTest.PercentError, 2))
//            };
//        }

//        private static PressureTest TranslatePressureTest(Prover.Core.Models.Instruments.VerificationTest vt)
//        {
//            if (vt.PressureTest == null) return new PressureTest();

//            return new PressureTest
//            {
//                ActualFactor = Convert.ToDouble(RoundTo(vt.PressureTest.ActualFactor, 4)),
//                EvcPressureFactor = Convert.ToDouble(RoundTo(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue, 4)),
//                GaugePressure = Convert.ToDouble(RoundTo(vt.PressureTest.GasGauge, 2)),
//                AtmosphericGauge = Convert.ToDouble(RoundTo(vt.PressureTest.AtmosphericGauge, 2)),
//                GasPressure = Convert.ToDouble(RoundTo(vt.PressureTest.GasPressure, 2)),
//                EvcGasPressure = Convert.ToDouble(RoundTo(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue, 2)),
//                PercentError = Convert.ToDouble(RoundTo(vt.PressureTest.PercentError, 2))
//            };
//        }

//        private static decimal RoundTo(decimal? value, int places)
//        {
//            if (value.HasValue)
//                return decimal.Round(value.Value, places);

//            return 0;
//        }
//    }
//}