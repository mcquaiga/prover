using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;

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

        [JsonConstructor]
        public VolumeTest(IEnumerable<ItemValue> items, string testInstrumentData, string driveTypeDiscriminator)
        {
            Items = items.ToList();
            DriveTypeDiscriminator = driveTypeDiscriminator;
            _testInstrumentData = testInstrumentData;

            OnInitializing();
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }

        [JsonIgnore]
        public IDriveType DriveType { get; private set; }

        public string TestInstrumentData
        {
            get { return AfterTestItems.Serialize(); }
            set { _testInstrumentData = value; }
        }

        public Instrument Instrument => VerificationTest?.Instrument;

        public Guid VerificationTestId { get; set; }

        [Required]
        public VerificationTest VerificationTest { get; set; }

        [NotMapped, JsonIgnore]
        public IEnumerable<ItemValue> AfterTestItems { get; set; }

        [NotMapped, JsonIgnore]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (EvcUncorrected != null && TrueUncorrected != 0 && TrueUncorrected.HasValue)
                {
                    var o = (EvcUncorrected - TrueUncorrected) / TrueUncorrected;
                    if (o != null)
                        return Math.Round((decimal) o * 100, 2);
                }

                return null;
            }
        }

        [NotMapped, JsonIgnore]
        public decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

        [NotMapped, JsonIgnore]
        public decimal? CorrectedPercentError
        {
            get
            {
                if (EvcCorrected != null && TrueCorrected != 0 && TrueCorrected != null)
                {
                    var o = (EvcCorrected - TrueCorrected) / TrueCorrected * 100;
                    if (o != null)
                        return Math.Round((decimal) o, 2);
                }

                return null;
            }
        }

        [NotMapped, JsonIgnore]
        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(Global.COR_ERROR_THRESHOLD) ?? false;

        [NotMapped, JsonIgnore]
        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(Global.UNCOR_ERROR_THRESHOLD) ?? false;

        [NotMapped, JsonIgnore]
        public new bool HasPassed
            => CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed && UnCorPulsesPassed && CorPulsesPassed
        ;

        public override decimal? PercentError { get; }
        public override decimal? ActualFactor { get; }

        [NotMapped, JsonIgnore]
        public int UncPulseCount
        {
            get
            {
                if (Instrument.PulseASelect() == "UncVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped, JsonIgnore]
        public int CorPulseCount
        {
            get
            {
                if (Instrument.PulseASelect() == "CorVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped, JsonIgnore]
        public bool UnCorPulsesPassed
        {
            get
            {               
                var result = AfterTestItems?.Uncorrected() - Items?.Uncorrected();
                if (result == null) return false;

                var expectedPulses = (int) result;
                var variance = expectedPulses - UncPulseCount;
                return variance.IsBetween(2);
            }
        }

        [NotMapped, JsonIgnore]
        public bool CorPulsesPassed
        {
            get
            {
                var result = AfterTestItems?.Corrected() - Items?.Corrected();
                if (result == null) return false;

                var expectedPulses = (int) result;
                var variance = expectedPulses - CorPulseCount;
                return variance.IsBetween(2);
            }
        }

        [NotMapped, JsonIgnore]
        public decimal? TrueCorrected
        {
            get
            {
                if (VerificationTest == null) return null;

                if (VerificationTest.Instrument.CompositionType == CorrectorType.T &&
                    VerificationTest.TemperatureTest != null)
                    return VerificationTest.TemperatureTest.ActualFactor *
                           DriveType.UnCorrectedInputVolume(AppliedInput);

                if (VerificationTest.Instrument.CompositionType == CorrectorType.P &&
                    VerificationTest.PressureTest != null)
                    return VerificationTest.PressureTest.ActualFactor *
                           DriveType.UnCorrectedInputVolume(AppliedInput);

                if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
                    return VerificationTest.PressureTest?.ActualFactor * VerificationTest.TemperatureTest?.ActualFactor *
                           VerificationTest.SuperFactorTest.SuperFactorSquared *
                           DriveType.UnCorrectedInputVolume(AppliedInput);

                return null;
            }
        }

        [NotMapped, JsonIgnore]
        public decimal? EvcCorrected => VerificationTest.Instrument.EvcCorrected(Items, AfterTestItems);

        [NotMapped, JsonIgnore]
        public decimal? EvcUncorrected => VerificationTest.Instrument.EvcUncorrected(Items, AfterTestItems);

        public string DriveTypeDiscriminator { get; set; }

        [NotMapped, JsonIgnore]
        public override InstrumentType InstrumentType => Instrument.InstrumentType;

        private void CreateDriveType()
        {
            if (string.IsNullOrEmpty(DriveTypeDiscriminator))
                DriveTypeDiscriminator = Instrument.Items?.GetItem(98)?.Description.ToLower() == "rotary"
                    ? "Rotary"
                    : "Mechanical";

            if (DriveType == null && !string.IsNullOrEmpty(DriveTypeDiscriminator) && Instrument != null)
                switch (DriveTypeDiscriminator.ToLower())
                {
                    case "rotary":
                        DriveType = new RotaryDrive(Instrument);
                        break;
                    case "mechanical":
                        DriveType = new MechanicalDrive(Instrument);
                        break;
                    default:
                        throw new NotSupportedException($"Drive type {DriveTypeDiscriminator} is not supported.");
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