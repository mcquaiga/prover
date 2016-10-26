using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
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
            _log.Debug($"Exporting Instrument object to MASA: {instrument.ToString()}");

            var qaRun = new DCRWebService.QARunEvcTestResult()
            {
                InstrumentType = instrument.InstrumentType.Name,
                InventoryCode = instrument.SiteNumber2.ToString(CultureInfo.InvariantCulture),
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
                    BasePressure = instrument.EvcBasePressure() ?? decimal.Zero,
                    PressureRange = instrument.EvcPressureRange() ?? decimal.MinusOne,
                    PressureUnits = instrument.PressureUnits(),
                    TransducerType = instrument.GetTransducerType().ToString(),
                    ProgrammedAtmosphericPressure = instrument.EvcAtmosphericPressure() ?? decimal.MinusOne
                },

                TemperatureInfo = new DCRWebService.TemperatureHeader
                {
                    BaseTemperature = instrument.EvcBaseTemperature() ?? decimal.MinusOne,
                    TemperatureRange = "-40 to 170 C",
                    TemperatureUnits = instrument.TemperatureUnits()
                },

                SuperFactorInfo = new DCRWebService.SuperFactorHeader
                {
                    CO2 = instrument.CO2() ?? decimal.MinusOne,
                    SpecGr = instrument.SpecGr() ?? decimal.MinusOne,
                    N2 = instrument.N2() ?? decimal.MinusOne,
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

                VerificationTests = instrument.VerificationTests.Select(vt => TranslateVerificationTest(vt)).ToArray()   ,
                IndexReading = 0,
                Comments = "Testing DCR Webservice",
                JobNumber = 27084,
                ProverNumber = "229",
                MeterClassCode = "EV",
                TestReason = "6" ,
                FieldMeterDesc = "SJB",
                SubmitRunIndicator = "Y"
            };
            
            return qaRun;
        }

        private static DCRWebService.VerificationTest TranslateVerificationTest(VerificationTest vt)
        {
            return new DCRWebService.VerificationTest
            {
                SequenceNumber = vt.TestNumber,
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
                AppliedInput = vt.VolumeTest.AppliedInput,
                EvcCorrected = vt.VolumeTest.EvcCorrected ?? -1,
                EvcUncorrected = vt.VolumeTest.EvcUncorrected ?? -1,
                CorPulseCount = vt.VolumeTest.CorPulseCount,
                UncPulseCount = vt.VolumeTest.UncPulseCount,
                PulseACount = vt.VolumeTest.PulseACount,
                PulseBCount = vt.VolumeTest.PulseBCount,
                TrueCorrected = vt.VolumeTest.TrueCorrected ?? -1,
                CorrectedPercentError = vt.VolumeTest.CorrectedPercentError ?? -1,
                UnCorrectedPercentError = vt.VolumeTest.UnCorrectedPercentError ?? -1               
            };
        }

        private static DCRWebService.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            if (vt.SuperFactorTest == null) return null;

            return new DCRWebService.SuperFactorTest
            {
                ActualFactor = vt.SuperFactorTest.ActualFactor ?? -999,
                EvcFactor = vt.SuperFactorTest.EvcUnsqrFactor ?? -999,
                EvcUnsqrFactor = vt.SuperFactorTest.EvcUnsqrFactor ?? -999,
                GaugePressure = vt.SuperFactorTest.GaugePressure ?? -999,
                GaugeTemperature = vt.SuperFactorTest.GaugeTemp,
                PercentError = vt.SuperFactorTest.PercentError ?? -999
            };
        }

        private static DCRWebService.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new DCRWebService.TemperatureTest
            {
                ActualFactor = vt.TemperatureTest.ActualFactor ?? decimal.Zero,
                GaugeTemperature = (decimal)vt.TemperatureTest.Gauge,
                EvcFactor = vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.Factor).NumericValue,
                EvcTemperature = vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.GasTemperature).NumericValue,
                PercentError = vt.TemperatureTest.PercentError ?? decimal.MinusOne
                
            };
        }

        private static DCRWebService.PressureTest TranslatePressureTest(VerificationTest vt)
        {
            if (vt.PressureTest == null) return null;

            return new DCRWebService.PressureTest
            {
                ActualFactor = vt.PressureTest.ActualFactor ?? decimal.Zero,
                GaugePressure = vt.PressureTest.GasGauge ?? decimal.MinusOne,
                AtmosphericGauge = vt.PressureTest.AtmosphericGauge ?? decimal.MinusOne,
                GasPressure = vt.PressureTest.GasPressure ?? decimal.MinusOne,
                EvcGasPressure = vt.PressureTest.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue,
                EvcPressureFactor = vt.PressureTest.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue,
                PercentError = vt.PressureTest.PercentError ?? decimal.MinusOne
                
            };
        }
    }
}
