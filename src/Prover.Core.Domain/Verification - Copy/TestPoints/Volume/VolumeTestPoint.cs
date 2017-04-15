using Prover.Domain.Verification.TestPoints.Volume.DriveTypes;

// ReSharper disable All

namespace Prover.Domain.Verification.TestPoints.Volume
{
    public partial class VolumeTestPoint : TestPointBase<IVolumeItems>
    {
        public VolumeTestPoint() : base(Guid.NewGuid())
        {
        }

        public VolumeTestPoint(IVolumeItems evcItems) : base(Guid.NewGuid(), evcItems)
        {
            SetDriveType();
        }

        public VolumeTestPoint(Guid id, double appliedInput, IVolumeItems preTestItems, IVolumeItems postTestItems)
            : base(id)
        {
            EvcItems = preTestItems;
            AppliedInput = appliedInput;
            PreTestItems = preTestItems;
            PostTestItems = postTestItems;

            SetDriveType();
        }

        public double AppliedInput { get; private set; }

        public IDriveType DriveType { get; private set; }
        public IVolumeItems PostTestItems { get; set; }
        public IVolumeItems PreTestItems { get; set; }

        public void SetDriveType()
        {
            if (EvcItems.DriveType == DriveTypeDescripter.Rotary)
                DriveType = new RotaryDrive(EvcItems);
            else
                DriveType = new MechanicalDrive(this);
        }
    }
}

//public int PulseACount { get; set; }
//public int PulseBCount { get; set; }
//public double PulseBScaling { get; }
//public int PulseCCount { get; set; }
//public double PulserAScaling { get; }
//public string PulserAUnits { get; }
//public string PulserBUnits { get; }