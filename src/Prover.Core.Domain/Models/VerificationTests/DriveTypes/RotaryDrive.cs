using System;
using Prover.Domain.Models.Instruments;
using Prover.Domain.Models.Instruments.Items;

namespace Prover.Domain.Models.VerificationTests.DriveTypes
{
    public class RotaryDrive : IDriveType
    {
        private readonly IVolumeItems _volumeItems;

        public RotaryDrive(IVolumeItems volumeItems)
        {
            _volumeItems = volumeItems;
            Meter = new MeterTest(_volumeItems);
        }

        public MeterTest Meter { get; set; }

        public string Discriminator => "Rotary";

        public bool HasPassed => Meter.MeterDisplacementHasPassed;

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return Meter.MeterDisplacement * appliedInput;
        }

        public int MaxUncorrectedPulses()
        {
            if (_volumeItems.UncorrectedMultiplier == 10)
                return Meter.MeterIndex.UnCorPulsesX10;

            if (_volumeItems.UncorrectedMultiplier == 100)
                return Meter.MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }
    }

    public class MeterTest
    {
        private readonly IVolumeItems _volumeItems;

        public MeterTest(IVolumeItems volumeItems)
        {
            _volumeItems = volumeItems;
            MeterIndex = MeterIndexInfo.Get(_volumeItems.MeterModelId);
        }

        public MeterIndexInfo MeterIndex { get; set; }

        public bool MeterDisplacementHasPassed
        {
            get { return false; // MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD); 
            }
        }

        public decimal MeterDisplacement
        {
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        }

        public decimal MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                    return Math.Round((_volumeItems.MeterDisplacement - MeterDisplacement) / MeterDisplacement * 100, 2);
                return 0;
            }
        }

        public string MeterTypeDescription
        {
            get { return MeterIndex.Description; }
        }
    }
}