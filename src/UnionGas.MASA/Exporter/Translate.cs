using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Prover.CommProtocol.Common.Items;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace UnionGas.MASA.Exporter
{
    public static class Translate
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static DCRWebService.QARunEvcTestResult RunTranslationForExport(Instrument instrument)
        {
            var qaRun = new DCRWebService.QARunEvcTestResult()
            {
                InstrumentType = instrument.InstrumentType.Name,
                InventoryCode = instrument.SiteNumber2.ToString(CultureInfo.InvariantCulture).PadLeft(7, '0'),
                TestDate = instrument.TestDateTime,
                DriveType = instrument.VolumeTest.DriveTypeDiscriminator,
                MeterType = instrument.VolumeTest.DriveTypeDiscriminator == "Rotary" ? 
                    (instrument.VolumeTest.DriveType as RotaryDrive).Meter.MeterTypeDescription : string.Empty,
                MeterDisplacement = instrument.VolumeTest.DriveTypeDiscriminator == "Rotary" ?
                    (instrument.VolumeTest.DriveType as RotaryDrive).Meter.MeterDisplacement : decimal.Zero,
                ConfirmedStatus = instrument.HasPassed ? "PASS" : "FAIL",
                FirmwareVersion = instrument.FirmwareVersion,
                SerialNumber = instrument.SerialNumber.ToString(),
                InstrumentData = instrument.InstrumentData,
                InstrumentComposition = instrument.CompositionType.ToString(),
                EmployeeId = instrument.EmployeeId,
               
                PressureInfo = new DCRWebService.PressureHeader
                {
                    BasePressure = instrument.EvcBasePressure().HasValue ? decimal.Round(instrument.EvcBasePressure().Value, 2) : decimal.Zero,
                    PressureRange = instrument.EvcPressureRange() ?? decimal.MinusOne,
                    PressureUnits = instrument.PressureUnits(),
                    TransducerType = instrument.GetTransducerType().ToString().Substring(0, 1),
                    ProgrammedAtmosphericPressure = instrument.EvcAtmosphericPressure().HasValue ? decimal.Round(instrument.EvcAtmosphericPressure().Value, 2) : decimal.MinusOne
                },

                TemperatureInfo = new DCRWebService.TemperatureHeader
                {
                    BaseTemperature = instrument.EvcBaseTemperature().HasValue ? decimal.Round(instrument.EvcBaseTemperature().Value, 2) : decimal.MinusOne,
                    TemperatureRange = "-40 to 170",
                    TemperatureUnits = $"deg{instrument.TemperatureUnits()}"
                },

                SuperFactorInfo = new DCRWebService.SuperFactorHeader
                {
                    CO2 = instrument.CO2().HasValue ? decimal.Round(instrument.CO2().Value, 4) : decimal.MinusOne,
                    SpecGr = instrument.SpecGr().HasValue ? decimal.Round(instrument.SpecGr().Value, 4) : decimal.MinusOne,
                    N2 = instrument.N2().HasValue ? decimal.Round(instrument.N2().Value, 4) : decimal.MinusOne,
                    FPVTable = "NX19"
                },

                VolumeInfo = new DCRWebService.VolumeHeader
                {
                    CorrectedMultiplierDescription = instrument.CorrectedMultiplierDescription(),
                    CorrectedMultiplierValue = instrument.CorrectedMultiplier().HasValue ? (int)instrument.CorrectedMultiplier().Value : 0,

                    UncorrectedMultiplierDescription = instrument.UnCorrectedMultiplierDescription(),
                    UncorrectedMultiplierValue = instrument.UnCorrectedMultiplier().HasValue ? (int)instrument.UnCorrectedMultiplier().Value : 0,

                    DriveRateDescription = instrument.DriveRateDescription(),

                    PulseASelect = instrument.PulseASelect(),
                    PulseBSelect = instrument.PulseBSelect()
                },

                VerificationTests = instrument.VerificationTests.Select(TranslateVerificationTest).OrderBy(t => t.SequenceNumber).ToArray(),
                IndexReading = 0,
                Comments = "Testing DCR Webservice",
                JobNumber = 27084,
                ProverNumber = "229", //
                MeterClassCode = "EV",
                TestReason = "6" ,
                FieldMeterDesc = "SJB",
                SubmitRunIndicator = "Y"
            };

            Task.Run(() =>
            {
                var objectString = JsonConvert.SerializeObject(qaRun, Formatting.Indented,
                    new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                _log.Debug($"Exporting Instrument object to MASA: {Environment.NewLine} {objectString}");
            }); 

            return qaRun;
        }

        private static DCRWebService.VerificationTest TranslateVerificationTest(VerificationTest vt)
        {
            return new DCRWebService.VerificationTest
            {
                SequenceNumber = vt.TestNumber + 1,
                CapacityLevelTypeCode = vt.TestNumber + 1 == 1 ? "L" : vt.TestNumber + 1 == 2 ? "M" : "H",
                Pressure = TranslatePressureTest(vt),
                Temperature = TranslateTemperatureTest(vt),
                SuperFactor = TranslateSuperFactorTest(vt),
                Volume = TranslateVolumeTest(vt)           
            };
        }

        private static DCRWebService.VolumeTest TranslateVolumeTest(VerificationTest vt)
        {
            if (vt.VolumeTest == null) return null;

            return new DCRWebService.VolumeTest
            {
                AppliedInput = RoundTo(vt.VolumeTest.AppliedInput, 2),
                EvcCorrected = RoundTo(vt.VolumeTest.EvcCorrected, 2),
                EvcUncorrected = RoundTo(vt.VolumeTest.EvcUncorrected, 2),
                CorPulseCount = vt.VolumeTest.CorPulseCount,
                UncPulseCount = vt.VolumeTest.UncPulseCount,
                PulseACount = vt.VolumeTest.PulseACount,
                PulseBCount = vt.VolumeTest.PulseBCount,
                TrueCorrected = RoundTo(vt.VolumeTest.TrueCorrected, 4),
                CorrectedPercentError = RoundTo(vt.VolumeTest.CorrectedPercentError, 2),
                UnCorrectedPercentError = RoundTo(vt.VolumeTest.UnCorrectedPercentError, 2)
                         
            };
        }

        private static DCRWebService.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            if (vt.SuperFactorTest == null) return null;

            return new DCRWebService.SuperFactorTest
            {
                ActualFactor = RoundTo(vt.SuperFactorTest.ActualFactor, 4),
                EvcFactor = RoundTo(vt.SuperFactorTest.EvcUnsqrFactor, 4),
                EvcUnsqrFactor = RoundTo(vt.SuperFactorTest.EvcUnsqrFactor, 4),
                GaugePressure = RoundTo(vt.SuperFactorTest.GaugePressure, 2),
                GaugeTemperature = RoundTo(vt.SuperFactorTest.GaugeTemp, 2),
                PercentError = RoundTo(vt.SuperFactorTest.PercentError, 2)
            };
        }

        private static DCRWebService.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new DCRWebService.TemperatureTest
            {
                ActualFactor = RoundTo(vt.TemperatureTest.ActualFactor, 4),
                GaugeTemperature = RoundTo((decimal)vt.TemperatureTest.Gauge, 2),
                EvcFactor = RoundTo(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.Factor).NumericValue, 4),
                EvcTemperature = RoundTo(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.GasTemperature).NumericValue, 2),
                PercentError = RoundTo(vt.TemperatureTest.PercentError, 2)

            };
        }

        private static DCRWebService.PressureTest TranslatePressureTest(VerificationTest vt)
        {
            if (vt.PressureTest == null) return null;

            return new DCRWebService.PressureTest
            {
                ActualFactor = RoundTo(vt.PressureTest.ActualFactor, 4),
                EvcPressureFactor = RoundTo(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue, 4),
                GaugePressure = RoundTo(vt.PressureTest.GasGauge, 2),
                AtmosphericGauge = RoundTo(vt.PressureTest.AtmosphericGauge, 2),
                GasPressure = RoundTo(vt.PressureTest.GasPressure, 2),
                EvcGasPressure = RoundTo(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue, 2),
                PercentError = RoundTo(vt.PressureTest.PercentError, 2)
            };
        }

        private static decimal RoundTo(decimal? value, int places)
        {
            if (value.HasValue)
                return decimal.Round(value.Value, places);

            return 0;
        }
    }
}
