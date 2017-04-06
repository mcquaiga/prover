using System;
using Prover.Domain.Models.Instruments;
using Prover.Domain.Models.VerificationTests.PTZ;

namespace Prover.Domain.Models.VerificationTests.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        private readonly VolumeTestPoint _volumeTest;

        public MechanicalDrive(VolumeTestPoint volumeTest)
        {
            _volumeTest = volumeTest;
            Energy = new Energy(_volumeTest);
        }

        public IInstrument Instrument { get; set; }

        public Energy Energy { get; set; }

        public string Discriminator => "Mechanical";

        public bool HasPassed => Energy.HasPassed;

        public int MaxUncorrectedPulses()
        {
            return 10;
            //var uncorPulseTable = SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits;
            //var uncorUnitValue = (int)Instrument.VolumeItems.DriveRate;

            //return uncorPulseTable.FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        decimal IDriveType.UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * Instrument.VolumeItems.DriveRate;
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
        private readonly VolumeTestPoint _volumeTest;

        public Energy(VolumeTestPoint volumeTest)
        {
            _volumeTest = volumeTest;
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
                var startEnergy = _volumeTest.PreTestItems.Energy;
                var endEnergy = _volumeTest.PostTestItems.Energy;

                return endEnergy - startEnergy;
            }
        }

        public string EnergyUnits => _volumeTest.PreTestItems.EnergyUnits;

        public decimal? ActualEnergy
        {
            get
            {
                var energyValue = _volumeTest.PostTestItems.EnergyGasValue;
                switch (EnergyUnits)
                {
                    case Therms:
                        return Math.Round(energyValue * _volumeTest.CorrectedEvcTotal) / 100000;
                    case Dktherms:
                        return Math.Round(energyValue * _volumeTest.CorrectedEvcTotal) / 1000000;
                    case GigaJoules:
                        break;
                }

                return null;
            }
        }
    }
}