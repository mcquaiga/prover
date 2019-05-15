using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Domain.Calculators.Helpers;
using Domain.Interfaces;
using Domain.Models.EvcVerifications.CorrectionTests;

namespace Domain.Models.EvcVerifications.DriveTypes
{
    public class Energy : TestRunBase<IEnergyItems>
    {
        public override decimal Actual
        {
            get
            {
                switch (StartEvcValues.EnergyUnits)

                {
                    case EnergyUnits.Therms:
                        return Round.Factor(EndEvcValues.EnergyGasValue * EvcCorrected / 100000);

                    case EnergyUnits.Dktherms:
                        return Math.Round(EndEvcValues.EnergyGasValue * EvcCorrected / 1000000);

                    case EnergyUnits.GigaJoules:
                        return Math.Round(EndEvcValues.EnergyGasValue * 0.028317m * EvcCorrected / 1000000);

                    case EnergyUnits.MegaJoules:
                        return Math.Round(EndEvcValues.EnergyGasValue * 0.028317m * EvcCorrected / 1000);

                    case EnergyUnits.KiloCals:
                        return Math.Round(EndEvcValues.EnergyGasValue * 0.0283168m * EvcCorrected);

                    default:
                        throw new Exception($"Energy units not supported: {StartEvcValues.EnergyUnits}");
                }
            }
        }

        public decimal EvcCorrected { get; set; }
        public override decimal Expected => EndEvcValues.EnergyReading - StartEvcValues.EnergyReading;
        public override decimal PassTolerance => Global.ENERGY_PASS_TOLERANCE;

        public Energy(IEnergyItems items)
        {
            _items = items;
        }

        private readonly IEnergyItems _items;
    }

    public class MechanicalDrive : IDriveType
    {
        public IDeviceWithValues Device { get; }

        public string Discriminator => Drives.Mechanical;
        public Energy Energy { get; set; }
        public bool HasPassed { get; }

        public MechanicalDrive(IMechanicalDeviceWithValues device)
        {
            Device = device;
            Energy = new Energy(device.Energy);
            _uncorUnits = Device.Volume.UncorrectedMultiplier;
        }

        public MechanicalDrive(IMechanicalDeviceWithValues device, List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits)
            : this(device)
        {
            _mechanicalUncorrectedTestLimits = mechanicalUncorrectedTestLimits;
        }

        public int MaxUncorrectedPulses()
        {
            return _mechanicalUncorrectedTestLimits?
                       .FirstOrDefault(x => x.CuFtValue == _uncorUnits)?.UncorrectedPulses ?? 10;
        }

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * Device.Volume.DriveRate;
        }

        private readonly List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;
        private readonly decimal _uncorUnits;
    }
}