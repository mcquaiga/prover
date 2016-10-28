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
                //MeterDisplacement = decimal.MinusOne,
                //instrument.VolumeTest.DriveTypeDiscriminator == "Rotary" ?
                  //  (instrument.VolumeTest.DriveType as RotaryDrive).Meter.MeterDisplacement : decimal.Zero,
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
                SequenceNumber = vt.TestNumber,
                CapacityLevelTypeCode = vt.TestNumber == 0 ? "L" : vt.TestNumber == 1 ? "M" : "H",
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
                AppliedInput = decimal.Round(vt.VolumeTest.AppliedInput, 2),
                EvcCorrected = vt.VolumeTest.EvcCorrected.HasValue ? decimal.Round(vt.VolumeTest.EvcCorrected.Value, 4) : -999,
                EvcUncorrected = vt.VolumeTest.EvcUncorrected.HasValue ? decimal.Round(vt.VolumeTest.EvcUncorrected.Value, 4) : -999,
                CorPulseCount = vt.VolumeTest.CorPulseCount,
                UncPulseCount = vt.VolumeTest.UncPulseCount,
                PulseACount = vt.VolumeTest.PulseACount,
                PulseBCount = vt.VolumeTest.PulseBCount,
                TrueCorrected = vt.VolumeTest.TrueCorrected.HasValue ? decimal.Round(vt.VolumeTest.TrueCorrected.Value, 4) : -999,
                CorrectedPercentError = vt.VolumeTest.CorrectedPercentError.HasValue ? decimal.Round(vt.VolumeTest.CorrectedPercentError.Value, 2) : -999,
                UnCorrectedPercentError = vt.VolumeTest.UnCorrectedPercentError.HasValue ? decimal.Round(vt.VolumeTest.UnCorrectedPercentError.Value, 2) : -999               
            };
        }

        private static DCRWebService.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            if (vt.SuperFactorTest == null) return null;

            return new DCRWebService.SuperFactorTest
            {
                ActualFactor = vt.SuperFactorTest.ActualFactor.HasValue ? decimal.Round(vt.SuperFactorTest.ActualFactor.Value, 4) : -999,
                EvcFactor = vt.SuperFactorTest.EvcUnsqrFactor.HasValue ? decimal.Round(vt.SuperFactorTest.EvcUnsqrFactor.Value, 4) : -999,
                EvcUnsqrFactor = vt.SuperFactorTest.EvcUnsqrFactor.HasValue ? decimal.Round(vt.SuperFactorTest.EvcUnsqrFactor.Value, 4) : -999,
                GaugePressure = vt.SuperFactorTest.GaugePressure.HasValue ? decimal.Round(vt.SuperFactorTest.GaugePressure.Value, 2) : -999,
                GaugeTemperature = decimal.Round(vt.SuperFactorTest.GaugeTemp, 2),
                PercentError = vt.SuperFactorTest.PercentError.HasValue ? decimal.Round(vt.SuperFactorTest.PercentError.Value, 2) : -999
            };
        }

        private static DCRWebService.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new DCRWebService.TemperatureTest
            {
                ActualFactor = vt.TemperatureTest.ActualFactor.HasValue ? decimal.Round(vt.TemperatureTest.ActualFactor.Value, 4) : decimal.Zero,
                GaugeTemperature = decimal.Round((decimal)vt.TemperatureTest.Gauge, 2),
                EvcFactor = decimal.Round(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.Factor).NumericValue, 4),
                EvcTemperature = decimal.Round(vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.GasTemperature).NumericValue, 2),
                PercentError = vt.TemperatureTest.PercentError.HasValue ? decimal.Round(vt.TemperatureTest.PercentError.Value, 2) : decimal.MinusOne
                
            };
        }

        private static DCRWebService.PressureTest TranslatePressureTest(VerificationTest vt)
        {
            if (vt.PressureTest == null) return null;

            return new DCRWebService.PressureTest
            {
                ActualFactor = vt.PressureTest.ActualFactor.HasValue ? decimal.Round(vt.PressureTest.ActualFactor.Value, 4) : decimal.Zero,
                GaugePressure = vt.PressureTest.GasGauge.HasValue ? decimal.Round(vt.PressureTest.GasGauge.Value, 2) : decimal.MinusOne,
                AtmosphericGauge = vt.PressureTest.AtmosphericGauge.HasValue ? decimal.Round(vt.PressureTest.AtmosphericGauge.Value, 2) : decimal.MinusOne,
                GasPressure = vt.PressureTest.GasPressure.HasValue ? decimal.Round(vt.PressureTest.GasPressure.Value, 2) : decimal.MinusOne,
                EvcGasPressure = decimal.Round(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue, 2),
                EvcPressureFactor = decimal.Round(vt.PressureTest.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue, 4),
                PercentError = vt.PressureTest.PercentError.HasValue ? decimal.Round(vt.PressureTest.PercentError.Value, 2) :  decimal.MinusOne
                
            };
        }
    }
}
