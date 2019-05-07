namespace Module.EvcVerification.Models.CorrectionTests
{
    using Core.Domain;
    using Core.Extensions;
    using Devices.Core.Interfaces.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public decimal AppliedInput { get; set; }

        public bool CorrectedHasPassed => CorrectedPercentError?.IsBetween(Global.COR_ERROR_THRESHOLD) ?? false;

        public decimal? CorrectedPercentError
        {
            get
            {
                //if (EvcCorrected.HasValue && TrueCorrected.HasValue && TrueCorrected != 0)
                //{
                //    var o = (EvcCorrected.Value - TrueCorrected.Value) / TrueCorrected.Value;
                //    return decimal.Round(o * 100, 2);
                //}

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

        public decimal PassTolerance => throw new NotImplementedException();

        //CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed &&
        //                       UnCorPulsesPassed && CorPulsesPassed;

        public ICollection<PulseOutputChannel> PulseOutputChannels { get; }

        public decimal? TotalCorrectionFactor
        {
            get
            {
                //if (VerificationTest == null) return null;

                //if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.T && VerificationTest.TemperatureTest != null)
                //    return VerificationTest.TemperatureTest.ActualFactor;

                //if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P && VerificationTest.PressureTest != null)
                //    return VerificationTest.PressureTest.ActualFactor;

                //if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
                //{
                //    return VerificationTest.PressureTest?.ActualFactor
                //          * VerificationTest.TemperatureTest?.ActualFactor
                //          * VerificationTest.SuperFactorTest.SuperFactorSquared;
                //}

                return null;
            }
        }

        public virtual decimal? TrueCorrected
        {
            get
            {
                // if (VerificationTest == null) return null;

                return TotalCorrectionFactor * DriveType.UnCorrectedInputVolume(AppliedInput);
            }
        }

        public virtual decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

        public bool UnCorPulsesPassed
        {
            get
            {
                return false;
                //var expectedPulses = (int?)(EndItems?.Uncorrected() - Items?.Uncorrected());
                //if (!expectedPulses.HasValue) return false;

                //var variance = expectedPulses - UncPulseCount;
                //return variance.IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
            }
        }

        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(Global.UNCOR_ERROR_THRESHOLD) ?? false;

        public decimal? UnCorrectedPercentError
        {
            get
            {
                //if (EvcUncorrected.HasValue && TrueUncorrected.HasValue && TrueUncorrected != 0)
                //{
                //    var o = (EvcUncorrected.Value - TrueUncorrected.Value) / TrueUncorrected.Value;
                //    return decimal.Round(o * 100, 2);
                //}

                return null;
            }
        }

        public int UncorrectedPulseTarget => DriveType.MaxUncorrectedPulses();

        /// <summary>
        /// The CreateDriveType
        /// </summary>
        /// <param name="mechanicalUncorrectedTestLimits">
        /// The mechanicalUncorrectedTestLimits <see cref="List{TestSettings.MechanicalUncorrectedTestLimit}"/>
        /// </param>
        //public void CreateDriveType()
        //{
        //    if (string.IsNullOrEmpty(DriveTypeDiscriminator))
        //    {
        //        if (Instrument.Items?.GetItem(182)?.NumericValue > 0)
        //        {
        //            DriveTypeDiscriminator = Drives.PulseInput;
        //        }
        //        else
        //        {
        //            DriveTypeDiscriminator = Instrument.Items?.GetItem(98)?.Description.ToLower() == Drives.Rotary.ToLower()
        //                ? Drives.Rotary
        //                : Drives.Mechanical;
        //        }
        //    }

        //    if (EvcDeviceType.Id == 12)
        //        DriveTypeDiscriminator = "Rotary";

        //    if (DriveType == null && !string.IsNullOrEmpty(DriveTypeDiscriminator) && VerificationTest != null)
        //    {
        //        switch (DriveTypeDiscriminator)
        //        {
        //            case Drives.Rotary:
        //                DriveType = new RotaryDrive(Instrument);
        //                break;

        //            case Drives.Mechanical:
        //                DriveType = new MechanicalDrive(Instrument, mechanicalUncorrectedTestLimits);
        //                break;

        //            case Drives.PulseInput:
        //                DriveType = new PulseInputSensor(Instrument);
        //                break;

        //            default:
        //                throw new NotSupportedException($"Drive type {DriveTypeDiscriminator} is not supported.");
        //        }
        //    }
        //    else
        //        throw new ArgumentNullException($"Could not determine drive type {DriveTypeDiscriminator}.");
        //}

        /// <summary>
        /// The OnInitializing
        /// </summary>
        //public override void OnInitializing()
        //{
        //    base.OnInitializing();

        //    CreateDriveType();

        //    if (!string.IsNullOrEmpty(_testInstrumentData))
        //    {
        //        var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_testInstrumentData);
        //        EndItems = ItemHelpers.LoadItems(Instrument.InstrumentType, afterItemValues);
        //    }
        //}
    }
}