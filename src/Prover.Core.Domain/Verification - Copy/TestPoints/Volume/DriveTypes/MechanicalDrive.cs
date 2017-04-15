namespace Prover.Domain.Verification.TestPoints.Volume.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        private readonly VolumeTestPoint _volumeTest;

        public MechanicalDrive(VolumeTestPoint volumeTest)
        {
            _volumeTest = volumeTest;
            Energy = new Energy(_volumeTest);
        }

        public string Discriminator => "Mechanical";

        public Energy Energy { get; set; }

        public bool HasPassed => false;

        public int MaxUncorrectedPulses()
        {
            return 10;
            //var uncorPulseTable = SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits;
            //var uncorUnitValue = (int)Instrument.VolumeItems.DriveRate;

            //return uncorPulseTable.FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        double IDriveType.UnCorrectedInputVolume(double appliedInput)
        {
            return appliedInput; // * Instrument.VolumeItems.DriveRate;
        }
    }

    public class Energy : IEnergyItems
    {
        private const string Dktherms = "DecaTherms";
        private const string GigaJoules = "GigaJoules";
        private const string KiloCals = "KiloCals";
        private const string KiloWattHours = "KiloWattHours";
        private const string MegaJoules = " MegaJoules";
        private const string Therms = "Therms";
        private readonly VolumeTestPoint _volumeTest;

        public Energy(VolumeTestPoint volumeTest)
        {
            _volumeTest = volumeTest;
        }

        public double? ActualEnergy
        {
            get
            {
                var energyValue = _volumeTest.PostTestItems.EnergyGasValue;
                switch (EnergyUnits)
                {
                    case Therms:
                        return Math.Round(energyValue * _volumeTest.CorrectedCalculator.EvcTotal) / 100000;
                    case Dktherms:
                        return Math.Round(energyValue * _volumeTest.CorrectedCalculator.EvcTotal) / 1000000;
                    case GigaJoules:
                        break;
                }

                return null;
            }
        }

        public double EnergyGasValue { get; set; }

        public double EnergyReading { get; set; }

        public string EnergyUnits => _volumeTest.PreTestItems.EnergyUnits;

        public double? EvcEnergy
        {
            get
            {
                var startEnergy = _volumeTest.PreTestItems.Energy;
                var endEnergy = _volumeTest.PostTestItems.Energy;

                return endEnergy - startEnergy;
            }
        }


        public double? PercentError
        {
            get
            {
                var error = (EvcEnergy - ActualEnergy) / ActualEnergy * 100;
                if (error != null)
                    return Math.Round(error.Value, 2);
                return null;
            }
        }

        string IEnergyItems.EnergyUnits { get; set; }
    }
}