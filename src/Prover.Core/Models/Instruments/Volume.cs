using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using NLog;
using Prover.Core.Extensions;
using Prover.Core.EVCTypes;

namespace Prover.Core.Models.Instruments
{
    public class VolumeTest : ProverTable
    {
        private Instrument _instrument;
        private string _driveTypeDiscriminator;

        public VolumeTest() { }

        public VolumeTest(VerificationTest verificationTest, IDriveType driveType)
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            _instrument = VerificationTest.Instrument;
            
            DriveType = driveType;
            DriveTypeDiscriminator = DriveType.Discriminator;
        }

        public VolumeTest(VerificationTest verificationTest, IDriveType driveType, Dictionary<int, string> afterTestItems) : this(verificationTest, driveType)
        {
            AfterTestItemValues = afterTestItems;
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }

        public IDriveType DriveType { get; set; }

        public string DriveTypeDiscriminator
        {
            get
            {
                return _driveTypeDiscriminator;
            }
            set
            {
                _driveTypeDiscriminator = value;
                if (DriveType == null)
                {
                    switch (_driveTypeDiscriminator)
                    {
                        case "Rotary":
                            DriveType = new RotaryDrive(this.VerificationTest.Instrument);
                            break;
                        case "Mechanical":
                            DriveType = new MechanicalDrive(this.VerificationTest.Instrument);
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Drive type {0} is not supported.", _driveTypeDiscriminator));
                    }
                }
            }
        }
        public string TestInstrumentData
        {
            get { return JsonConvert.SerializeObject(AfterTestItemValues); }
            set
            {
                AfterTestItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        public Guid VerificationTestId { get; set; }
        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        [NotMapped]
        public Dictionary<int, string> AfterTestItemValues { get; set; }

        [NotMapped]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (ItemValues == null || AfterTestItemValues == null) return null;

                if (DriveType.UnCorrectedInputVolume(AppliedInput) != 0 && DriveType.UnCorrectedInputVolume(AppliedInput) != null)
                {
                    return Math.Round((decimal)(((EvcUncorrected - DriveType.UnCorrectedInputVolume(AppliedInput) / DriveType.UnCorrectedInputVolume(AppliedInput) * 100))), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public decimal? CorrectedPercentError
        {
            get
            {

                if (ItemValues == null || AfterTestItemValues == null) return null;

                if (TrueCorrected != 0 && TrueCorrected != null)
                {
                    return Math.Round((decimal)(((EvcCorrected - TrueCorrected) / TrueCorrected) * 100), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public decimal EvcCorrected
        {
            get
            {
                return VerificationTest.Instrument.EvcCorrected(ItemValues, AfterTestItemValues).Value;
            }
        }

        [NotMapped]
        public decimal EvcUncorrected
        {
            get
            {
                return VerificationTest.Instrument.EvcUncorrected(ItemValues, AfterTestItemValues).Value;
            }
        }

        [NotMapped]
        public bool CorrectedHasPassed
        {
            get { return CorrectedPercentError.IsBetween(Global.COR_ERROR_THRESHOLD); }
        }

        [NotMapped]
        public bool UnCorrectedHasPassed
        {
            get { return (UnCorrectedPercentError.IsBetween(Global.UNCOR_ERROR_THRESHOLD)); }
        }

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                return CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed;
            }
        }

        [NotMapped]
        public int UncPulseCount
        {

            get
            {
                if (_instrument.PulseASelect() == "UncVol")
                    return PulseACount;

                return PulseBCount;
            }

        }

        [NotMapped]
        public int CorPulseCount
        {
            get
            {
                if (_instrument.PulseASelect() == "CorVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped]
        public decimal? TrueCorrected
        {
            get
            {
                if (VerificationTest.Instrument.CorrectorType == CorrectorType.TemperatureOnly && VerificationTest.TemperatureTest != null)
                {
                    return (VerificationTest.TemperatureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                if (VerificationTest.Instrument.CorrectorType == CorrectorType.PressureOnly && VerificationTest.PressureTest != null)
                {
                    return (VerificationTest.PressureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }
                else if (VerificationTest.Instrument.CorrectorType == CorrectorType.PressureTemperature)
                {
                    return (VerificationTest.PressureTest.ActualFactor * VerificationTest.TemperatureTest.ActualFactor * VerificationTest.SuperFactorTest.SuperFactorSquared * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                return null;
            }
        }
    }
}
