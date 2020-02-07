//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Devices.Core;
//using Devices.Core.Interfaces.Items;
//using Domain.Calculators.Helpers;
//using Domain.Entities;
//using Domain.Interfaces;

//namespace Domain.Models.EvcVerifications.CorrectionTests
//{
//    public class CorrectedVolume : TestRunBase<IVolumeItems>, IAssertPassFail
//    {
//        public override decimal Actual { get; }

//        public override decimal Expected { get; }

//        public override decimal PassTolerance => Global.COR_ERROR_THRESHOLD;

//        public decimal TotalCorrectionFactor
//        {
//            get
//            {
//                if (De.CompositionType == EvcCorrectorType.T && VerificationTest.TemperatureTest != null)
//                    return VerificationTest.TemperatureTest.ActualFactor;

//                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P && VerificationTest.PressureTest != null)
//                    return VerificationTest.PressureTest.ActualFactor;

//                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
//                {
//                    return VerificationTest.PressureTest?.ActualFactor
//                           * VerificationTest.TemperatureTest?.ActualFactor
//                           * VerificationTest.SuperFactorTest.SuperFactorSquared;
//                }

//                return null;
//            }
//        }

//        public CorrectedVolume(CompositionType compositionType)
//        {
//        }
//    }

//    public class UncorrectedVolume : TestRunBase<IVolumeItems>, IAssertPassFail
//    {
//        public override decimal Actual { get; }

//        public override decimal Expected => _expected;

//        public override decimal PassTolerance => Global.UNCOR_ERROR_THRESHOLD;

//        public UncorrectedVolume(IDriveType driveType)
//        {
//            _driveType = driveType;
//        }

//        public void CalculateTotal(decimal appliedInput)
//        {
//            _expected = _driveType.UnCorrectedInputVolume(appliedInput);
//        }

//        private readonly IDriveType _driveType;
//        private decimal _expected;
//    }

//    /// <summary>
//    /// Defines the <see cref="VolumeTest"/>
//    /// </summary>
//    public class VolumeTest : TestRunBase<IVolumeItems>, IAssertPassFail
//    {
//        public override decimal Actual { get; }

//        public decimal AppliedInput { get; set; }

//        public decimal? CorrectedPercentError
//        {
//            get
//            {
//                //if (EvcCorrected.HasValue && TrueCorrected.HasValue && TrueCorrected != 0)
//                //{
//                //    var o = (EvcCorrected.Value - TrueCorrected.Value) / TrueCorrected.Value;
//                //    return decimal.Round(o * 100, 2);
//                //}

//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets or sets the DriveType
//        /// </summary>
//        public IDriveType DriveType { get; set; }

//        public bool Passed
//        {
//            get
//            {
//                return PulseOutputChannels.All(p => p.Passed);
//            }
//        }

//        public ICollection<PulseOutputChannel> PulseOutputChannels { get; }

//        //CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed &&
//        //                       UnCorPulsesPassed && CorPulsesPassed;
//        public decimal? TotalCorrectionFactor
//        {
//            get
//            {
//                if (De.CompositionType == EvcCorrectorType.T && VerificationTest.TemperatureTest != null)
//                    return VerificationTest.TemperatureTest.ActualFactor;

//                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P && VerificationTest.PressureTest != null)
//                    return VerificationTest.PressureTest.ActualFactor;

//                if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
//                {
//                    return VerificationTest.PressureTest?.ActualFactor
//                          * VerificationTest.TemperatureTest?.ActualFactor
//                          * VerificationTest.SuperFactorTest.SuperFactorSquared;
//                }

//                return null;
//            }
//        }

//        public virtual decimal? TrueCorrected => TotalCorrectionFactor * DriveType.UnCorrectedInputVolume(AppliedInput);
//        public virtual decimal? TrueUncorrected => DriveType?.UnCorrectedInputVolume(AppliedInput);

//        public bool UnCorPulsesPassed
//        {
//            get
//            {
//                return false;
//                //var expectedPulses = (int?)(EndItems?.Uncorrected() - Items?.Uncorrected());
//                //if (!expectedPulses.HasValue) return false;

//                //var variance = expectedPulses - UncPulseCount;
//                //return variance.IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
//            }
//        }

//        public UncorrectedVolume Uncorrected { get; set; }

//        public bool UnCorrectedHasPassed => UnCorrectedPercentError?.IsBetween(Global.UNCOR_ERROR_THRESHOLD) ?? false;

//        public decimal? UnCorrectedPercentError
//        {
//            get
//            {
//                //if (EvcUncorrected.HasValue && TrueUncorrected.HasValue && TrueUncorrected != 0)
//                //{
//                //    var o = (EvcUncorrected.Value - TrueUncorrected.Value) / TrueUncorrected.Value;
//                //    return decimal.Round(o * 100, 2);
//                //}

//                return null;
//            }
//        }

//        public int UncorrectedPulseTarget => DriveType.MaxUncorrectedPulses();

//        public VolumeTest(ICollection<PulseOutputChannel> pulseOutputChannels)
//        {
//            PulseOutputChannels = pulseOutputChannels;
//        }

//        /// <summary>
//        /// The CreateDriveType
//        /// </summary>
//        /// <param name="mechanicalUncorrectedTestLimits">
//        /// The mechanicalUncorrectedTestLimits <see cref="List{TestSettings.MechanicalUncorrectedTestLimit}"/>
//        /// </param>
//        //public void CreateDriveType()
//        //{
//        //    if (string.IsNullOrEmpty(DriveTypeDiscriminator))
//        //    {
//        //        if (Instrument.Items?.GetItem(182)?.NumericValue > 0)
//        //        {
//        //            DriveTypeDiscriminator = Drives.PulseInput;
//        //        }
//        //        else
//        //        {
//        //            DriveTypeDiscriminator = Instrument.Items?.GetItem(98)?.Description.ToLower() == Drives.Rotary.ToLower()
//        //                ? Drives.Rotary
//        //                : Drives.Mechanical;
//        //        }
//        //    }

//        //    if (EvcDeviceType.Id == 12)
//        //        DriveTypeDiscriminator = "Rotary";

//        //    if (DriveType == null && !string.IsNullOrEmpty(DriveTypeDiscriminator) && VerificationTest != null)
//        //    {
//        //        switch (DriveTypeDiscriminator)
//        //        {
//        //            case Drives.Rotary:
//        //                DriveType = new RotaryDrive(Instrument);
//        //                break;

//        //            case Drives.Mechanical:
//        //                DriveType = new MechanicalDrive(Instrument, mechanicalUncorrectedTestLimits);
//        //                break;

//        //            case Drives.PulseInput:
//        //                DriveType = new PulseInputSensor(Instrument);
//        //                break;

//        //            default:
//        //                throw new NotSupportedException($"Drive type {DriveTypeDiscriminator} is not supported.");
//        //        }
//        //    }
//        //    else
//        //        throw new ArgumentNullException($"Could not determine drive type {DriveTypeDiscriminator}.");
//        //}

//        /// <summary>
//        /// The OnInitializing
//        /// </summary>
//        //public override void OnInitializing()
//        //{
//        //    base.OnInitializing();

//        //    CreateDriveType();

//        //    if (!string.IsNullOrEmpty(_testInstrumentData))
//        //    {
//        //        var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_testInstrumentData);
//        //        EndItems = ItemHelpers.LoadItems(Instrument.InstrumentType, afterItemValues);
//        //    }
//        //}
//    }
//}