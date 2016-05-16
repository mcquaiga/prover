﻿using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionGas.MASA.Models;

namespace UnionGas.MASA
{
    public class Translate
    {
        public Translate(Instrument instrument)
        {
            this.Instrument = instrument;
        }

        public Instrument Instrument { get; private set; }

        public EvcQARun RunTranslationForExport()
        {

            var verificationTests = new List<EvcQARun.VerificationTest>();
            
            foreach(var vt in Instrument.VerificationTests)
            {
                verificationTests.Add(TranslateVerificationTest(vt));
            }

            var qaRun = new EvcQARun
            {
                CompanyNumber = Instrument.SiteNumber2.ToString(),
                DateTime = Instrument.TestDateTime,
                DriveType = "Rotary",
                ConfirmedStatus = Instrument.HasPassed ? "PASS" : "FAIL",
                FirmwareVersion = Instrument.FirmwareVersion,
                InstrumentType = Instrument.TypeString,
                SerialNumber = Instrument.SerialNumber.ToString(),
                InstrumentData = Instrument.InstrumentData,
                InstrumentComposition = Instrument.CorrectorType.ToString(),

                PressureInfo = new EvcQARun.PressureHeader
                {
                    BasePressure = Instrument.EvcBasePressure().Value,
                    PressureRange = Instrument.EvcPressureRange().Value,
                    PressureUnits = Instrument.PressureUnits(),
                    TransducerType = Instrument.GetTransducerType().ToString(),
                    ProgrammedAtmosphericPressure = Instrument.EvcAtmosphericPressure().Value
                },
                
                TemperatureInfo = new EvcQARun.TemperatureHeader
                {
                    BaseTemperature = Instrument.EvcBaseTemperature().Value,
                    TemperatureRange = "-40 to 170 C",
                    TemperatureUnits = Instrument.TemperatureUnits()
                },

                SuperFactorInfo = new EvcQARun.SuperFactorHeader
                {
                    CO2 = null,
                    SpecGr = null,
                    N2 = null,
                    FPVTable = "NX19"
                },

                VolumeInfo = new EvcQARun.VolumeHeader
                {
                    CorrectedMultiplierDescription = Instrument.CorrectedMultiplierDescription(),
                    CorrectedMultiplierValue = (int)Instrument.CorrectedMultiplier(),

                    UncorrectedMultiplierDescription = Instrument.UnCorrectedMultiplierDescription(),
                    UncorrectedMultiplierValue = (int)Instrument.CorrectedMultiplier(), 
                    
                    DriveRateDescription = Instrument.DriveRateDescription(),
                    
                    PulseASelect = Instrument.PulseASelect(),
                    PulseBSelect = Instrument.PulseBSelect()                                       
                },
                                
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
                EvcCorrected = vt.VolumeTest.ItemValues.Corrected().Value,
                EvcUncorrected = vt.VolumeTest.ItemValues.Uncorrected().Value,
                
            }
        }

        private static EvcQARun.VerificationTest.SuperFactorTest TranslateSuperFactorTest(VerificationTest vt)
        {
            throw new NotImplementedException();
        }

        private static EvcQARun.VerificationTest.TemperatureTest TranslateTemperatureTest(VerificationTest vt)
        {
            if (vt.TemperatureTest == null) return null;

            return new EvcQARun.VerificationTest.TemperatureTest
            {
                ActualFactor = vt.TemperatureTest.ActualFactor.Value,
                GaugeTemperature = (decimal)vt.TemperatureTest.Gauge,
                EvcFactor = vt.TemperatureTest.ItemValues.EvcTemperatureFactor().Value,
                EvcTemperature = vt.TemperatureTest.ItemValues.EvcTemperatureReading().Value,
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

                EvcGasPressure = vt.PressureTest.ItemValues.EvcGasPressure().Value,
                EvcPressureFactor = vt.PressureTest.ItemValues.EvcGasPressure().Value,
                PercentError = vt.PressureTest.PercentError.Value
            };
        }
    }
}
