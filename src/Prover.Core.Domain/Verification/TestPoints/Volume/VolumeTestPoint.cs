using System;
using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Volume.DriveTypes;
using Prover.Shared.Enums;

// ReSharper disable All

namespace Prover.Domain.Verification.TestPoints.Volume
{
    public class VolumeTestPoint : TestPointBase<IVolumeItems>
    {
        private CorrectedVolumeCalculator _correctedCalculator;
        private UncorrectedVolumeCalculator _uncorrectedCalculator;

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

        public IVolumeCalculator CorrectedCalculator => _correctedCalculator;

        public IDriveType DriveType { get; private set; }
        public IVolumeItems PostTestItems { get; set; }

        public IVolumeItems PreTestItems { get; set; }
        public IVolumeCalculator UncorrectedCalculator => _uncorrectedCalculator;

        public void BeginTest(IVolumeItems startTestVolumeItems)
        {
            PreTestItems = startTestVolumeItems;
            PostTestItems = null;

            _uncorrectedCalculator = null;
            _correctedCalculator = null;
            AppliedInput = 0;
        }

        public void CompleteTest(IVolumeItems endTestVolumeItems, double appliedInput,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            PostTestItems = endTestVolumeItems;
            Update(appliedInput, temperatureFactor, pressureFactor, superFactor);
        }

        public void SetDriveType()
        {
            if (EvcItems.DriveType == DriveTypeDescripter.Rotary)
                DriveType = new RotaryDrive(EvcItems);
            else
                DriveType = new MechanicalDrive(this);
        }

        public void Update(double appliedInput, double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            if (PreTestItems == null || PostTestItems == null) return;

            AppliedInput = appliedInput;

            _uncorrectedCalculator = new UncorrectedVolumeCalculator(DriveType,
                EvcItems.UncorrectedMultiplier,
                AppliedInput,
                PreTestItems.UncorrectedReading,
                PostTestItems.UncorrectedReading);

            _correctedCalculator = new CorrectedVolumeCalculator(EvcItems.CorrectedMultiplier, _uncorrectedCalculator.Calculated,
                PreTestItems.CorrectedReading, PostTestItems.CorrectedReading,
                temperatureFactor, pressureFactor, superFactor);
        }

        public void Update(IVolumeItems preTestItems, IVolumeItems postTestItems, double? appliedInput,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            PreTestItems = preTestItems;
            PostTestItems = postTestItems;

            Update(appliedInput ?? AppliedInput, temperatureFactor, pressureFactor, superFactor);
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