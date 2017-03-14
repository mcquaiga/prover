using System;
using Prover.CommProtocol.Common.Items;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(TestRun testRun)
        {
            TestRun = testRun;
            Energy = new Energy(testRun);
        }

        public Energy Energy { get; set; }

        public TestRun TestRun { get; }

        public string Discriminator => "Mechanical";

        public bool HasPassed => Energy.HasPassed;

        public int MaxUncorrectedPulses()
        {
            //var uncorPulseTable = SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits;
            var uncorUnitValue = (int) TestRun.ItemValues.GetItem(98).NumericValue;

            return 10; // uncorPulseTable.FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * TestRun.ItemValues.GetItem(98).NumericValue;
        }
    }

    public class Energy
    {
        private const string Therms = "Therms";
        private const string Dktherms = "DecaTherms";
        private const string MegaJoules = " MegaJoules";
        private const string GigaJoules = "GigaJoules";
        private const string KiloCals = "KiloCals";
        private const string KiloWattHours = "KiloWattHours";

        private readonly TestRun _testRun;

        public Energy(TestRun testRun)
        {
            _testRun = testRun;
        }

        public bool HasPassed => PercentError < 1 && PercentError > -1;

        public decimal? PercentError
        {
            get
            {
                var error = (EvcEnergy - ActualEnergy) / ActualEnergy * 100;
                if (error != null)
                    return decimal.Round(error.Value, 2);
                return null;
            }
        }

        public decimal? EvcEnergy
        {
            get
            {
                var startEnergy = _testRun.VolumeTest.PreTestValues.GetItem(140)?.NumericValue;
                var endEnergy = _testRun.VolumeTest.PostTestValues?.GetItem(140)?.NumericValue;
                if (endEnergy != null && startEnergy != null)
                    return endEnergy.Value - startEnergy.Value;

                return null;
            }
        }

        public string EnergyUnits => _testRun.ItemValues.GetItem(141).Description;

        public decimal? ActualEnergy
        {
            get
            {
                var energyValue = _testRun.Items.GetItem(142).NumericValue;
                switch (EnergyUnits)
                {
                    case Therms:
                        if (_testRun.VolumeTest.EvcCorrected.HasValue)
                            return Math.Round(energyValue * _testRun.VolumeTest.EvcCorrected.Value) / 100000;
                        break;
                    case Dktherms:

                    case GigaJoules:
                        break;
                }

                return null;
            }
        }
    }
}