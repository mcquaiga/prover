using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.Shared.Enums;

namespace Prover.Core.Models.Instruments
{
    public sealed class VolumeTest : BaseVerificationTest
    {
        private string _testInstrumentData;

        public VolumeTest()
        {
        }

        public VolumeTest(VerificationTest verificationTest)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsVolumeTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            CreateDriveType();
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
        public decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

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
        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(Global.COR_ERROR_THRESHOLD) ?? false;

        [NotMapped]
        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(Global.UNCOR_ERROR_THRESHOLD) ?? false;

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
        public decimal? TrueCorrected
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

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.T &&
                    VerificationTest.TemperatureTest != null)
                    return VerificationTest.TemperatureTest.ActualFactor;

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P &&
                    VerificationTest.PressureTest != null)
                    return VerificationTest.PressureTest.ActualFactor;

                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
                    return VerificationTest.PressureTest?.ActualFactor *
                           VerificationTest.TemperatureTest?.ActualFactor *
                           VerificationTest.SuperFactorTest.SuperFactorSquared;

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

        private void CreateDriveType()
        {
            if (string.IsNullOrEmpty(DriveTypeDiscriminator))
            {
                DriveTypeDiscriminator = Instrument.Items?.GetItem(98)?.Description.ToLower() == "rotary"
                    ? "Rotary"
                    : "Mechanical";

                if (InstrumentType.Id == 12)
                    DriveTypeDiscriminator = "Rotary";                
            }
                

            if (DriveType == null && DriveTypeDiscriminator != null && VerificationTest != null)
            { 
                switch (DriveTypeDiscriminator)
                {
                    case "Rotary":
                        DriveType = new RotaryDrive(Instrument);
                        break;
                    case "Mechanical":
                        DriveType = new MechanicalDrive(Instrument);
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