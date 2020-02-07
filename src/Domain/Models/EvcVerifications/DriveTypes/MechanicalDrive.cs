using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.DriveTypes;
using Domain.Interfaces;
using Domain.Models.EvcVerifications.CorrectionTests;

namespace Domain.Models.EvcVerifications.DriveTypes
{
    public class EnergyTest : TestRunBase<IEnergyItems>
    {
        public decimal EvcCorrected { get; set; }
        public decimal Expected => EndValues.EnergyReading - StartValues.EnergyReading;
        public override decimal PassTolerance => Global.ENERGY_PASS_TOLERANCE;

        public EnergyTest(IEnergyItems items)
        {
            _items = items;
        }

        private readonly IEnergyItems _items;
    }

    //public class MechanicalDrive : IDriveType
    //{
    //    public string Discriminator => Drives.Mechanical;
    //    public Energy Energy { get; set; }

    //    public MechanicalDrive(IEnergyItems energyItems)
    //    {
    //        Energy = new Energy(energyItems);
    //    }

    //    public int MaxUncorrectedPulses()
    //    {
    //        return _mechanicalUncorrectedTestLimits?
    //                   .FirstOrDefault(x => x.CuFtValue == _uncorUnits)?.UncorrectedPulses ?? 10;
    //    }

    //    public decimal UnCorrectedInputVolume(decimal appliedInput)
    //    {
    //        return appliedInput * Volume.DriveRate;
    //    }

    //    private readonly List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;
    //}
}