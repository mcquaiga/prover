using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.Settings;
using Prover.Core.Shared.Enums;

namespace Prover.Core.Models.Instruments
{
    public class VolumeTest : BaseVerificationTest
    {
        public decimal UncorrectedErrorThreshold;
        public decimal CorrectedErrorThreshold;        

        private string _testInstrumentData;

        private VolumeTest() { }

        public static VolumeTest Create(VerificationTest verificationTest, TestSettings testSettings)
        {
            var volume = new VolumeTest()
            {
                Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsVolumeTest == true).ToList(),
                VerificationTest = verificationTest,
                VerificationTestId = verificationTest.Id,
                UncorrectedErrorThreshold = testSettings.UncorrectedErrorThreshold,
                CorrectedErrorThreshold = testSettings.CorrectedErrorThreshold
            };

            volume.CreateDriveType(testSettings.MechanicalUncorrectedTestLimits);

            return volume;
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }

        public IDriveType DriveType { get; set; }

        [NotMapped]
        public IEnumerable<ItemValue> AfterTestItems { get; set; }
        public string TestInstrumentData
        {
            get => AfterTestItems.Serialize();
            set => _testInstrumentData = value;
        }

        public Instrument Instrument => VerificationTest.Instrument;

        [NotMapped]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (EvcUncorrected.HasValue && TrueUncorrected.HasValue && TrueUncorrected != 0 )
                {
                    var o = (EvcUncorrected.Value - TrueUncorrected.Value) / TrueUncorrected.Value;
                    return decimal.Round(o * 100, 2);
                }

                return null;
            }
        }

        [NotMapped]
        public virtual decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

        [NotMapped]
        public decimal? CorrectedPercentError
        {
            get
            {
                if (EvcCorrected.HasValue && TrueCorrected.HasValue && TrueCorrected != 0 )
                {
                    var o = (EvcCorrected.Value - TrueCorrected.Value) / TrueCorrected.Value;
                    return decimal.Round(o * 100, 2);
                }

                return null;
            }
        }

        [NotMapped]
        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(CorrectedErrorThreshold) ?? false;

        [NotMapped]
        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(UncorrectedErrorThreshold) ?? false;

        [NotMapped]
        public new bool HasPassed => CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed &&
                                     UnCorPulsesPassed && CorPulsesPassed;

        public override decimal? PercentError { get; }
        public override decimal? ActualFactor { get; }
        public override decimal? EvcFactor { get; }

        [NotMapped]
        public int UncPulseCount
        {
            get
            {
                if (Instrument.PulseASelect() == "UncVol")
                    return PulseACount;

                return PulseBCount;
            }
            set
            {
                if (Instrument.PulseASelect() == "UncVol")
                    PulseACount = value;
                else
                    PulseBCount = value;
            }
        }

        [NotMapped]
        public int CorPulseCount
        {
            get
            {
                if (Instrument.PulseASelect() == "CorVol")
                    return PulseACount;

                return PulseBCount;
            }
            set
            {
                if (Instrument.PulseASelect() == "CorVol")
                    PulseACount = value;
                else
                    PulseBCount = value;
            }
        }

        [NotMapped]
        public bool UnCorPulsesPassed
        {
            get
            {
                var expectedPulses = (int?) (AfterTestItems?.Uncorrected() - Items?.Uncorrected());
                if (!expectedPulses.HasValue) return false;

                var variance = expectedPulses - UncPulseCount;
                return variance.IsBetween(2);
            }
        }

        [NotMapped]
        public bool CorPulsesPassed
        {
            get
            {
                var expectedPulses = (int?) (AfterTestItems?.Corrected() - Items.Corrected());
                if (!expectedPulses.HasValue) return false;

                var variance = expectedPulses - CorPulseCount;
                return variance.IsBetween(2);
            }
        }

        [NotMapped]
        public virtual decimal? TrueCorrected
        {
            get
            {
                if (VerificationTest == null) return null;

                return TotalCorrectionFactor * DriveType.UnCorrectedInputVolume(AppliedInput);
            }
        }

        [NotMapped]
        public decimal? TotalCorrectionFactor
        {
            get
            {
                if (VerificationTest == null) return null;

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.T && VerificationTest.TemperatureTest != null)
                    return VerificationTest.TemperatureTest.ActualFactor;

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P && VerificationTest.PressureTest != null)
                    return VerificationTest.PressureTest.ActualFactor;

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
                {
                    return VerificationTest.PressureTest?.ActualFactor
                          * VerificationTest.TemperatureTest?.ActualFactor
                          * VerificationTest.SuperFactorTest.SuperFactorSquared;
                }

                return null;
            }
        }

        [NotMapped]
        public decimal? EvcCorrected => VerificationTest.Instrument.EvcCorrected(Items, AfterTestItems);

        [NotMapped]
        public decimal? EvcUncorrected => VerificationTest.Instrument.EvcUncorrected(Items, AfterTestItems);

        public string DriveTypeDiscriminator { get; set; }

        [NotMapped]
        public override InstrumentType InstrumentType => Instrument.InstrumentType;

        private void CreateDriveType(List<TestSettings.MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits = null)
        {
            if (string.IsNullOrEmpty(DriveTypeDiscriminator))
            {
                if (Instrument.Items?.GetItem(182)?.NumericValue > 0)
                {
                    DriveTypeDiscriminator = Drives.PulseInput;
                }
                else
                {
                    DriveTypeDiscriminator = Instrument.Items?.GetItem(98)?.Description.ToLower() == Drives.Rotary.ToLower()
                        ? Drives.Rotary
                        : Drives.Mechanical;
                }                
            }

            if (InstrumentType.Id == 12)
	            DriveTypeDiscriminator = "Rotary";                

            if (DriveType == null && !string.IsNullOrEmpty(DriveTypeDiscriminator) && VerificationTest != null)
            { 
                switch (DriveTypeDiscriminator)
                {
                    case Drives.Rotary:
                        DriveType = new RotaryDrive(Instrument);
                        break;
                    case Drives.Mechanical:
                        DriveType = new MechanicalDrive(Instrument, mechanicalUncorrectedTestLimits);
                        break;
                    case Drives.PulseInput:
                        DriveType = new PulseInputSensor(Instrument);
                        break;
                    default:
                        throw new NotSupportedException($"Drive type {DriveTypeDiscriminator} is not supported.");
                }               
            }
            else
                throw new ArgumentNullException($"Could not determine drive type {DriveTypeDiscriminator}.");
        }

        public override void OnInitializing()
        {
            base.OnInitializing();

            CreateDriveType();

            if (!string.IsNullOrEmpty(_testInstrumentData))
            {
                var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_testInstrumentData);
                AfterTestItems = ItemHelpers.LoadItems(Instrument.InstrumentType, afterItemValues);
            }
        }
    }
}