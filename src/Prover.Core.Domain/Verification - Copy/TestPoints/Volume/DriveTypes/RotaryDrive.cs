namespace Prover.Domain.Verification.TestPoints.Volume.DriveTypes
{
    public class RotaryDrive : IDriveType
    {
        private readonly IVolumeItems _volumeItems;

        public RotaryDrive(IVolumeItems volumeItems)
        {
            _volumeItems = volumeItems;
            Meter = new MeterTest(_volumeItems);
        }

        public string Discriminator => "Rotary";

        public bool HasPassed => Meter.MeterDisplacementHasPassed;

        public MeterTest Meter { get; set; }

        public int MaxUncorrectedPulses()
        {
            if (_volumeItems.UncorrectedMultiplier == 10)
                return Meter.MeterIndex.UnCorPulsesX10;

            if (_volumeItems.UncorrectedMultiplier == 100)
                return Meter.MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }

        public double UnCorrectedInputVolume(double appliedInput)
        {
            return Meter.MeterDisplacement * appliedInput;
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

        public double MeterDisplacement
        {
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        }

        public bool MeterDisplacementHasPassed
        {
            get { return false; // MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD); 
            }
        }

        public double MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                    return Math.Round((_volumeItems.MeterDisplacement - MeterDisplacement) / MeterDisplacement * 100, 2);
                return 0;
            }
        }

        public MeterIndexInfo MeterIndex { get; set; }

        public string MeterTypeDescription
        {
            get { return MeterIndex.Description; }
        }
    }
}