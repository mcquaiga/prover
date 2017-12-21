using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prover.Core.Settings;
using Prover.Core.Shared.Domain;
using Prover.Core.Shared.Enums;

namespace Prover.Core.Models.Instruments
{
    public sealed class VerificationTest : EntityWithId
    {
        public VerificationTest()
        {
        }

        public VerificationTest(int testNumber, Instrument instrument)
        {
            TestNumber = testNumber;
            Instrument = instrument;
            InstrumentId = Instrument.Id;
        }

        public static List<VerificationTest> Create(Instrument instrument, TestSettings testSettings)
        {
            var results = new List<VerificationTest>();
            foreach (var vt in testSettings.TestPoints)
            {
                var verificationTest = new VerificationTest(vt.Level, instrument);

                if (instrument.CompositionType == EvcCorrectorType.P)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, vt.PressureGaugePercent);
                }

                if (instrument.CompositionType == EvcCorrectorType.T)
                {
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, vt.TemperatureGauge);
                }

                if (instrument.CompositionType == EvcCorrectorType.PTZ)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, vt.PressureGaugePercent);
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, vt.TemperatureGauge);
                    verificationTest.SuperFactorTest = new SuperFactorTest(verificationTest);
                }

                if (vt.IsVolumeTest)
                {
                    verificationTest.VolumeTest = VolumeTest.Create(verificationTest, testSettings);

                    if (instrument.InstrumentType.Name == "TOC")
                    {
                        verificationTest.FrequencyTest = new FrequencyTest(verificationTest);
                    }
                }

                results.Add(verificationTest);
            }

            return results;
        }

        public int TestNumber { get; set; }

        public Guid InstrumentId { get; set; }

        [Required]
        public Instrument Instrument { get; set; }

        public PressureTest PressureTest { get; set; }
        public TemperatureTest TemperatureTest { get; set; }
        public VolumeTest VolumeTest { get; set; }
        public FrequencyTest FrequencyTest { get; set; }

        [NotMapped]
        public string TestLevel => $"L{TestNumber + 1}";

        [NotMapped]
        public SuperFactorTest SuperFactorTest { get; set; }

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                if (Instrument.CompositionType == EvcCorrectorType.T && TemperatureTest != null)
                    return TemperatureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == EvcCorrectorType.P && PressureTest != null)
                    return PressureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == EvcCorrectorType.PTZ && PressureTest != null &&
                    TemperatureTest != null)
                    return TemperatureTest.HasPassed && PressureTest.HasPassed &&
                           (VolumeTest == null || VolumeTest.HasPassed);

                return false;
            }
        }

        public override void OnInitializing()
        {
            if (Instrument.CompositionType == EvcCorrectorType.PTZ)
                SuperFactorTest = new SuperFactorTest(this);
        }
    }
}