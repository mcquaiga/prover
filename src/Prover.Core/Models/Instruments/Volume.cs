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
    public class VolumeTest : InstrumentTable
    {
        private Instrument _instrument;
        public IDriveType DriveType { get; set; }

        public VolumeTest() { }

        public VolumeTest(VerificationTest verificationTest) : base(verificationTest.Instrument.Items.CopyItemsByFilter(i => i.IsVolume == true))
        {
            VerificationTest = verificationTest;
            _instrument = VerificationTest.Instrument;
            
            AfterTestItems = _instrument.Items.CopyItemsByFilter(x => x.IsVolumeTest == true);

            DriveType = new RotaryDrive(_instrument);
            DriveTypeDiscriminator = DriveType.Discriminator;
        }

        public VolumeTest(VerificationTest verificationTest, InstrumentItems afterTestItems) : this(verificationTest)
        {
            AfterTestItems = afterTestItems;
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }
        [NotMapped]
        public VerificationTest VerificationTest { get; set; }
        public InstrumentItems AfterTestItems { get; set; }
        public string DriveTypeDiscriminator { get; set; }
        public string TestInstrumentData
        {
            get { return JsonConvert.SerializeObject(AfterTestItems.InstrumentValues); }
            set
            {
                Items.InstrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public decimal UnCorrectedPercentError
        {
            get
            {
                if (DriveType.UnCorrectedInputVolume(AppliedInput) != 0 && DriveType.UnCorrectedInputVolume(AppliedInput) != null)
                {
                    return Math.Round((decimal)(((_instrument.EvcUncorrected(Items, AfterTestItems) - DriveType.UnCorrectedInputVolume(AppliedInput) / DriveType.UnCorrectedInputVolume(AppliedInput) * 100))), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public decimal CorrectedPercentError
        {
            get
            {
                if (TrueCorrected != 0 && TrueCorrected != null)
                {
                    return Math.Round((decimal)(((_instrument.EvcCorrected(Items, AfterTestItems) - TrueCorrected) / TrueCorrected) * 100), 2);
                }
                else
                {
                    return 0;
                }
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
                return CorrectedHasPassed && UnCorrectedHasPassed; //TODO: && MeterDisplacementHasPassed;
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
                if (_instrument.CorrectorType == CorrectorType.TemperatureOnly && VerificationTest.TemperatureTest != null)
                {
                    return (VerificationTest.TemperatureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                if (_instrument.CorrectorType == CorrectorType.PressureOnly && VerificationTest.PressureTest != null)
                {
                    return (VerificationTest.PressureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }
                else if (_instrument.CorrectorType == CorrectorType.PressureTemperature)
                {
                    return (VerificationTest.PressureTest.ActualFactor * VerificationTest.TemperatureTest.ActualFactor * VerificationTest.SuperFactorTest.SuperFactorSquared * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                return null;
            }
        }
    }
}
