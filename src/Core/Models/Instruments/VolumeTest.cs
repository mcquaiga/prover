namespace Prover.Core.Models.Instruments
{
    using Prover.Core.DriveTypes;
    using Prover.Core.Models.Instruments.DriveTypes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="VolumeTest" />
    /// </summary>
    public class VolumeTest : BaseVerificationTest
    {
        #region Fields

        /// <summary>
        /// Defines the _testInstrumentData
        /// </summary>
        private string _testInstrumentData;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="VolumeTest"/> class from being created.
        /// </summary>
        private VolumeTest()
        {
        }

        public VolumeTest(VerificationTest verificationTest, List<TestSettings.MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsVolumeTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = verificationTest.Id;

            CreateDriveType(mechanicalUncorrectedTestLimits);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override decimal? ActualFactor { get; }

        /// <summary>
        /// Gets or sets the AfterTestItems
        /// </summary>
        [NotMapped]
        public IEnumerable<ItemValue> AfterTestItems { get; set; }

        /// <summary>
        /// Gets or sets the AppliedInput
        /// </summary>
        public decimal AppliedInput { get; set; }

        /// <summary>
        /// Gets or sets the CorPulseCount
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether CorPulsesPassed
        /// </summary>
        [NotMapped]
        public bool CorPulsesPassed
        {
            get
            {
                var expectedPulses = (int?)(AfterTestItems?.Corrected() - Items.Corrected());
                if (!expectedPulses.HasValue) return false;

                var variance = expectedPulses - CorPulseCount;
                return variance.IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
            }
        }

        /// <summary>
        /// Gets a value indicating whether CorrectedHasPassed
        /// </summary>
        [NotMapped]
        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(Global.COR_ERROR_THRESHOLD) ?? false;

        /// <summary>
        /// Gets the CorrectedPercentError
        /// </summary>
        [NotMapped]
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

        [NotMapped]
        public int UncorrectedPulseTarget => DriveType.MaxUncorrectedPulses();

        /// <summary>
        /// Gets or sets the DriveType
        /// </summary>
        public IDriveType DriveType { get; set; }

        /// <summary>
        /// Gets or sets the DriveTypeDiscriminator
        /// </summary>
        public string DriveTypeDiscriminator { get; set; }

        /// <summary>
        /// Gets the EvcCorrected
        /// </summary>
        [NotMapped]
        public decimal? EvcCorrected => VerificationTest.Instrument.EvcCorrected(Items, AfterTestItems);

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override decimal? EvcFactor { get; }

        /// <summary>
        /// Gets the EvcUncorrected
        /// </summary>
        [NotMapped]
        public decimal? EvcUncorrected => VerificationTest.Instrument.EvcUncorrected(Items, AfterTestItems);

        /// <summary>
        /// Gets a value indicating whether HasPassed
        /// </summary>
        [NotMapped]
        public new bool HasPassed => CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed &&
                                     UnCorPulsesPassed && CorPulsesPassed;

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        public Instrument Instrument => VerificationTest.Instrument;

        /// <summary>
        /// Gets the InstrumentType
        /// </summary>
        [NotMapped]
        public override InstrumentType InstrumentType => Instrument.InstrumentType;

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

        /// <summary>
        /// Gets or sets the TestInstrumentData
        /// </summary>
        public string TestInstrumentData { get => AfterTestItems.Serialize(); set => _testInstrumentData = value; }

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
                var expectedPulses = (int?)(AfterTestItems?.Uncorrected() - Items?.Uncorrected());
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

        /// <summary>
        /// Gets or sets the UncPulseCount
        /// </summary>
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

        #endregion

        #region Methods      

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
                AfterTestItems = ItemHelpers.LoadItems(Instrument.InstrumentType, afterItemValues);
            }
        }

        /// <summary>
        /// The CreateDriveType
        /// </summary>
        /// <param name="mechanicalUncorrectedTestLimits">The mechanicalUncorrectedTestLimits<see cref="List{TestSettings.MechanicalUncorrectedTestLimit}"/></param>
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

        #endregion
    }
}
