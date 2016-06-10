using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionGas.MASA.Models;

namespace UnionGas.MASA
{
    public static class Translate
    {
        public static EvcQARun RunTranslationForExport(Instrument instrument)
        {
            var verificationTests = new List<EvcQARun.VerificationTest>();
            
            foreach(var vt in instrument.VerificationTests)
            {
                verificationTests.Add(TranslateVerificationTest(vt));
            }

            var qaRun = new EvcQARun
            {
                CompanyNumber = instrument.SiteNumber2.ToString(),
                DateTime = instrument.TestDateTime,
                DriveType = "Rotary",
                ConfirmedStatus = instrument.HasPassed ? "PASS" : "FAIL",
                FirmwareVersion = instrument.FirmwareVersion,
                InstrumentType = instrument.InstrumentTypeString,
                SerialNumber = instrument.SerialNumber.ToString(),
                InstrumentData = instrument.InstrumentData,
                InstrumentComposition = instrument.CompositionType.ToString(),

                //PressureInfo = new EvcQARun.PressureHeader
                //{
                //    BasePressure = instrument.EvcBasePressure().Value,
                //    PressureRange = instrument.EvcPressureRange().Value,
                //    PressureUnits = instrument.PressureUnits(),
                //    TransducerType = instrument.GetTransducerType().ToString(),
                //    ProgrammedAtmosphericPressure = instrument.EvcAtmosphericPressure().Value
                //},
                
                TemperatureInfo = new EvcQARun.TemperatureHeader
                {
                    BaseTemperature = instrument.EvcBaseTemperature().Value,
                    TemperatureRange = "-40 to 170 C",
                    TemperatureUnits = instrument.TemperatureUnits()
                },

                SuperFactorInfo = new EvcQARun.SuperFactorHeader
                {
                    CO2 = instrument.CO2().Value,
                    SpecGr = instrument.SpecGr().Value,
                    N2 = instrument.N2().Value,
                    FPVTable = "NX19"
                },

                VolumeInfo = new EvcQARun.VolumeHeader
                {
                    CorrectedMultiplierDescription = instrument.CorrectedMultiplierDescription(),
                    CorrectedMultiplierValue = (int)instrument.CorrectedMultiplier(),

                    UncorrectedMultiplierDescription = instrument.UnCorrectedMultiplierDescription(),
                    UncorrectedMultiplierValue = (int)instrument.CorrectedMultiplier(), 
                    
                    DriveRateDescription = instrument.DriveRateDescription(),
                    
                    PulseASelect = instrument.PulseASelect(),
                    PulseBSelect = instrument.PulseBSelect()                                       
                },
                
                VerificationTests = verificationTests              
            };

            return qaRun;
        }

        private static EvcQARun.VerificationTest TranslateVerificationTest(VerificationTest vt)
        {
            return new EvcQARun.VerificationTest
            {
                Pressure = TranslatePressureTest(vt),
                Temperature = TranslateTemperatureTest(vt),
                SuperFactor = TranslateSuperFactorTest(vt),
                Volume = TranslateVolumeTest(vt)
            };
        }

        private static EvcQARun.VerificationTest.VolumeTest TranslateVolumeTest(VerificationTest vt)
        {
            if (vt.VolumeTest == null) return null;

            return new EvcQARun.VerificationTest.VolumeTest
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

        private static EvcQARun.VerificationTest.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            if (vt.SuperFactorTest == null) return null;

            return new EvcQARun.VerificationTest.SuperFactorTest
            {
                ActualFactor = vt.SuperFactorTest.ActualFactor.Value,
                EvcFactor = vt.SuperFactorTest.EvcUnsqrFactor.Value,
                EVCUnsqrFactor = vt.SuperFactorTest.EvcUnsqrFactor.Value,
                GaugePressure = vt.SuperFactorTest.GaugePressure.Value,
                GaugeTemp = vt.SuperFactorTest.GaugeTemp,
                PercentError = vt.SuperFactorTest.PercentError.Value
            };
        }

        private static EvcQARun.VerificationTest.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new EvcQARun.VerificationTest.TemperatureTest
            {
                ActualFactor = vt.TemperatureTest.ActualFactor.Value,
                GaugeTemperature = (decimal)vt.TemperatureTest.Gauge,
                //EvcFactor = vt.TemperatureTest.Items.GetItem.Value,
                //EvcTemperature = vt.TemperatureTest.ItemValues.EvcTemperatureReading().Value,
                PercentError = vt.TemperatureTest.PercentError.Value
            };
        }

        private static EvcQARun.VerificationTest.PressureTest TranslatePressureTest(VerificationTest vt)
        {
            if (vt.PressureTest == null) return null;

            return new EvcQARun.VerificationTest.PressureTest
            {
                ActualFactor = vt.PressureTest.ActualFactor.Value,
                GaugePressure = vt.PressureTest.GasGauge.Value,
                AtmosphericGauge = vt.PressureTest.AtmosphericGauge.Value,
                GasPressure = vt.PressureTest.GasPressure.Value,

                //EvcGasPressure = vt.PressureTest.ItemValues.EvcGasPressure().Value,
                //EvcPressureFactor = vt.PressureTest.ItemValues.EvcGasPressure().Value,
                PercentError = vt.PressureTest.PercentError.Value
            };
        }
    }
}
