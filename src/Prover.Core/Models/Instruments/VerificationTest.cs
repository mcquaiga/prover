using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.Common;
using Prover.Core.Settings;
using Prover.Core.Shared.Domain;
using Prover.Core.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Core.Models.Instruments
{
    public sealed class VerificationTest : EntityWithId
    {
        private VerificationTest() { }

        public VerificationTest(Instrument instrument, int testLevel)
        {
            TestNumber = testLevel;
            Instrument = instrument;
            InstrumentId = Instrument.Id;
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
                var volumePass = VolumeTest == null || VolumeTest.HasPassed;

                if (Instrument.CompositionType == EvcCorrectorType.T && TemperatureTest != null)
                {
                    return TemperatureTest.HasPassed && volumePass;
                }

                if (Instrument.CompositionType == EvcCorrectorType.P && PressureTest != null)
                {
                    return PressureTest.HasPassed && volumePass;
                }

                if (Instrument.CompositionType == EvcCorrectorType.PTZ && PressureTest != null && TemperatureTest != null)
                {
                    return TemperatureTest.HasPassed && PressureTest.HasPassed && SuperFactorTest.HasPassed && volumePass;
                }

                return false;
            }
        }

        public override void OnInitializing()
        {
            base.OnInitializing();

            if (Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                SuperFactorTest = new SuperFactorTest(this);
            }
        }

        public static VerificationTest Create(Instrument instrument, TestSettings testSettings, TestSettings.TestPointSetting tp)
        {
            var vt = new VerificationTest(instrument, tp.Level);
            if (instrument.CompositionType == EvcCorrectorType.P)
            {
                vt.PressureTest = new PressureTest(vt, tp.PressureGaugePercent);
            }

            if (instrument.CompositionType == EvcCorrectorType.T)
            {
                vt.TemperatureTest = new TemperatureTest(vt, tp.TemperatureGauge);
            }

            if (instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                vt.PressureTest = new PressureTest(vt, tp.PressureGaugePercent);
                vt.TemperatureTest = new TemperatureTest(vt, tp.TemperatureGauge);
                vt.SuperFactorTest = new SuperFactorTest(vt);
            }

            if (tp.IsVolumeTest)
            {
                var volume = new VolumeTest(vt, testSettings.MechanicalUncorrectedTestLimits);

                if (instrument.InstrumentType == HoneywellInstrumentTypes.Toc)
                {
                    vt.FrequencyTest = new FrequencyTest(vt);
                }

                vt.VolumeTest = volume;
            }

            return vt;
        }
    }
}