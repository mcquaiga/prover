namespace Module.EvcVerification.Models.CorrectionTests
{
    using Core.Domain;
    using Core.Extensions;
    using Devices.Core.Interfaces.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Module.EvcVerification.CorrectionTestDefinition;

    public class PulseOutputChannel : IAssertPassFail, ICompareTestResults<int>
    {
        public int Actual { get; }

        public int Expected { get; }

        public bool Passed => Math.Abs(Expected - Actual).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);

        public decimal PercentVariance => (Expected - Actual) / Actual * 100;
    }

    /// <summary>
    /// Defines the <see cref="VolumeTest"/>
    /// </summary>
    public class VolumeTest : BaseEntity, IAssertPassFail
    {
        /// <summary>
        /// Defines the _testInstrumentData
        /// </summary>
        private string _testInstrumentData;

        /// <summary>
        /// Prevents a default instance of the <see cref="VolumeTest"/> class from being created.
        /// </summary>
        private VolumeTest()
        {
        }

        public VolumeTest(CorrectionTestPoint c)
        {
            VerificationTest = verificationTest;
        }

        public decimal AppliedInput { get; set; }

        public ICollection<Volume>
        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(Global.COR_ERROR_THRESHOLD) ?? false;

        public decimal? CorrectedPercentError
        {
            get
            {
                if (EvcCorrected.HasValue && TrueCorrected.HasValue && TrueCorrected != 0)
                {
                    var o = (EvcCorrected.Value - TrueCorrected.Value) / TrueCorrected.Value;
                    return decimal.Round(o * 100, 2);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the DriveType
        /// </summary>
        public IDriveType DriveType { get; set; }

        /// <summary>
        /// Gets or sets the DriveTypeDiscriminator
        /// </summary>
        public string DriveTypeDiscriminator { get; set; }

        public IVolumeItems EndItems { get; set; }

        public bool Passed
        {
            get
            {
                return PulseOutputChannels.All(p => p.Passed);
            }
        }

        //CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed &&
        //                       UnCorPulsesPassed && CorPulsesPassed;

        public decimal PassTolerance => throw new NotImplementedException();

        /// <summary>
        /// Gets the PercentError
        /// </summary>
        public override decimal? PercentError { get; }

        /// <summary>
        /// Gets or sets the PulseACount
        /// </summary>
        public int PulseACount { get; set; }

        /// <summary>
        /// Gets or sets the PulseBCount
        /// </summary>
        public int PulseBCount { get; set; }

        public ICollection<PulseOutputChannel> PulseOutputChannels { get; }

        /// <summary>
        /// Gets or sets the TestInstrumentData
        /// </summary>
        public string TestInstrumentData { get => EndItems.Serialize(); set => _testInstrumentData = value; }

        /// <summary>
        /// Gets the TotalCorrectionFactor
        /// </summary>
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

        /// <summary>
        /// Gets the TrueCorrected
        /// </summary>
        [NotMapped]
        public virtual decimal? TrueCorrected
        {
            get
            {
                if (VerificationTest == null) return null;

                return TotalCorrectionFactor * DriveType.UnCorrectedInputVolume(AppliedInput);
            }
        }

        /// <summary>
        /// Gets the TrueUncorrected
        /// </summary>
        [NotMapped]
        public virtual decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

        /// <summary>
        /// Gets a value indicating whether UnCorPulsesPassed
        /// </summary>
        [NotMapped]
        public bool UnCorPulsesPassed
        {
            get
            {
                var expectedPulses = (int?)(EndItems?.Uncorrected() - Items?.Uncorrected());
                if (!expectedPulses.HasValue) return false;

                var variance = expectedPulses - UncPulseCount;
                return variance.IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
            }
        }

        /// <summary>
        /// Gets a value indicating whether UnCorrectedHasPassed
        /// </summary>
        [NotMapped]
        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(Global.UNCOR_ERROR_THRESHOLD) ?? false;

        /// <summary>
        /// Gets the UnCorrectedPercentError
        /// </summary>
        [NotMapped]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (EvcUncorrected.HasValue && TrueUncorrected.HasValue && TrueUncorrected != 0)
                {
                    var o = (EvcUncorrected.Value - TrueUncorrected.Value) / TrueUncorrected.Value;
                    return decimal.Round(o * 100, 2);
                }

                return null;
            }
        }

        [NotMapped]
        public int UncorrectedPulseTarget => DriveType.MaxUncorrectedPulses();

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

        decimal ICompareFactors.ActualFactor => throw new NotImplementedException();
        public decimal? EvcCorrected => VerificationTest.Instrument.EvcCorrected(Items, EndItems);
        decimal ICompareFactors.EvcFactor => throw new NotImplementedException();
        public decimal? EvcUncorrected => VerificationTest.Instrument.EvcUncorrected(Items, EndItems);

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        public EvcVerification Instrument => VerificationTest.Instrument;

        /// <summary>
        /// The CreateDriveType
        /// </summary>
        /// <param name="mechanicalUncorrectedTestLimits">
        /// The mechanicalUncorrectedTestLimits <see cref="List{TestSettings.MechanicalUncorrectedTestLimit}"/>
        /// </param>
        public void CreateDriveType(List<TestSettings.MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits = null)
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

            if (EvcDeviceType.Id == 12)
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

        /// <summary>
        /// The OnInitializing
        /// </summary>
        public override void OnInitializing()
        {
            base.OnInitializing();

            CreateDriveType();

            if (!string.IsNullOrEmpty(_testInstrumentData))
            {
                var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_testInstrumentData);
                EndItems = ItemHelpers.LoadItems(Instrument.InstrumentType, afterItemValues);
            }
        }
    }
}