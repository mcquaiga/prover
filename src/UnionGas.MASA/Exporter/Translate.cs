using System;
using System.Collections.Generic;
using System.Globalization;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace UnionGas.MASA.Exporter
{
    public static class Translate
    {
        public static DCRWebService.QARunEvcTestResult RunTranslationForExport(Instrument instrument)
        {
            var verificationTests = new List<DCRWebService.VerificationTest>();
            
            foreach(var vt in instrument.VerificationTests)
            {
                verificationTests.Add(TranslateVerificationTest(vt));
            }

            var qaRun = new DCRWebService.QARunEvcTestResult()
            {
                InventoryCode = instrument.SiteNumber2.ToString(CultureInfo.InvariantCulture),
                TestDate = instrument.TestDateTime,
                DriveType = "Rotary",
                ConfirmedStatus = instrument.HasPassed ? "PASS" : "FAIL",
                FirmwareVersion = instrument.FirmwareVersion,
                InstrumentType = instrument.InstrumentTypeString,
                SerialNumber = instrument.SerialNumber.ToString(),
                InstrumentData = instrument.InstrumentData,
                InstrumentComposition = instrument.CompositionType.ToString(),

                PressureInfo = new DCRWebService.PressureHeader
                {
                    BasePressure = instrument.EvcBasePressure().HasValue ? instrument.EvcBasePressure().Value : Decimal.Zero,
                    PressureRange = instrument.EvcPressureRange().Value,
                    PressureUnits = instrument.PressureUnits(),
                    TransducerType = instrument.GetTransducerType().ToString(),
                    ProgrammedAtmosphericPressure = instrument.EvcAtmosphericPressure().Value
                },

                TemperatureInfo = new DCRWebService.TemperatureHeader
                {
                    BaseTemperature = instrument.EvcBaseTemperature().Value,
                    TemperatureRange = "-40 to 170 C",
                    TemperatureUnits = instrument.TemperatureUnits()
                },

                SuperFactorInfo = new DCRWebService.SuperFactorHeader
                {
                    CO2 = instrument.CO2().Value,
                    SpecGr = instrument.SpecGr().Value,
                    N2 = instrument.N2().Value,
                    FPVTable = "NX19"
                },

                VolumeInfo = new DCRWebService.VolumeHeader
                {
                    CorrectedMultiplierDescription = instrument.CorrectedMultiplierDescription(),
                    CorrectedMultiplierValue = (int)instrument.CorrectedMultiplier(),

                    UncorrectedMultiplierDescription = instrument.UnCorrectedMultiplierDescription(),
                    UncorrectedMultiplierValue = (int)instrument.CorrectedMultiplier(),

                    DriveRateDescription = instrument.DriveRateDescription(),

                    PulseASelect = instrument.PulseASelect(),
                    PulseBSelect = instrument.PulseBSelect()
                },

                VerificationTests = verificationTests.ToArray()              
            };

            return qaRun;
        }

        private static DCRWebService.VerificationTest TranslateVerificationTest(VerificationTest vt)
        {
            return new DCRWebService.VerificationTest
            {
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
                EvcCorrected = vt.VolumeTest.EvcCorrected.Value,
                EvcUncorrected = vt.VolumeTest.EvcUncorrected.Value,
                CorPulseCount = vt.VolumeTest.CorPulseCount,
                UncPulseCount = vt.VolumeTest.UncPulseCount,
                PulseACount = vt.VolumeTest.PulseACount,
                PulseBCount = vt.VolumeTest.PulseBCount,
                TrueCorrected = vt.VolumeTest.TrueCorrected.Value,
                CorrectedPercentError = vt.VolumeTest.CorrectedPercentError.Value,
                UnCorrectedPercentError = vt.VolumeTest.UnCorrectedPercentError.Value
            };
        }

        private static DCRWebService.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            if (vt.SuperFactorTest == null) return null;

            return new DCRWebService.SuperFactorTest
            {
                ActualFactor = vt.SuperFactorTest.ActualFactor.Value,
                EvcFactor = vt.SuperFactorTest.EvcUnsqrFactor.Value,
                EvcUnsqrFactor = vt.SuperFactorTest.EvcUnsqrFactor.Value,
                GaugePressure = vt.SuperFactorTest.GaugePressure.Value,
                GaugeTemperature = vt.SuperFactorTest.GaugeTemp,
                PercentError = vt.SuperFactorTest.PercentError.Value
            };
        }

        private static DCRWebService.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new DCRWebService.TemperatureTest
            {
                ActualFactor = vt.TemperatureTest.ActualFactor.Value,
                GaugeTemperature = (decimal)vt.TemperatureTest.Gauge,
                EvcFactor = vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.Factor).NumericValue,
                EvcTemperature = vt.TemperatureTest.Items.GetItem(ItemCodes.Temperature.GasTemperature).NumericValue,
                PercentError = vt.TemperatureTest.PercentError.Value
            };
        }

        private static DCRWebService.PressureTest TranslatePressureTest(VerificationTest vt)
        {
            if (vt.PressureTest == null) return null;

            return new DCRWebService.PressureTest
            {
                ActualFactor = vt.PressureTest.ActualFactor.Value,
                GaugePressure = vt.PressureTest.GasGauge.Value,
                AtmosphericGauge = vt.PressureTest.AtmosphericGauge.Value,
                GasPressure = vt.PressureTest.GasPressure.Value,
                EvcGasPressure = vt.PressureTest.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue,
                EvcPressureFactor = vt.PressureTest.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue,
                PercentError = vt.PressureTest.PercentError.Value
            };
        }
    }
}
