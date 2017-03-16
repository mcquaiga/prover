using System;

namespace Prover.Domain.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(Energy energy, decimal meterIndexRate, int maxUncorrectedPulses)
        {
            MeterIndexRate = meterIndexRate;
            MaxUncorrectedPulses = maxUncorrectedPulses;
            Energy = energy;
        }

        public decimal MeterIndexRate { get; }
        public int MaxUncorrectedPulses { get; }

        public Energy Energy { get; set; }

        public string Discriminator => "Mechanical";

        public bool HasPassed => Energy.HasPassed;

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * MeterIndexRate;
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

        public bool HasPassed => PercentError < 1 && PercentError > -1;

        public decimal? PercentError 
            => EnergyCalculated != 0 ? (EnergyTotal - EnergyCalculated) / EnergyCalculated * 100 
                : default(decimal?);

        public decimal CorrectedVolume { get; set; }
        public decimal EnergyStart { get; set; }
        public decimal EnergyEnd { get; set; }

        public decimal EnergyTotal
            => EnergyEnd - EnergyStart;

        public string EnergyUnits { get; set; }

        public decimal EnergyCalculated
        {
            get
            {
                switch (EnergyUnits)
                {
                    case Therms:
                        return Math.Round(GasEnergyValue * CorrectedVolume) / 100000;
                    case Dktherms:
                        throw new NotSupportedException("DKTherms energy calculations not supported.");
                    case GigaJoules:
                        throw new NotSupportedException("GigaJoules energy calculations not supported.");
                }

                return 0m;
            }
        }

        public decimal GasEnergyValue { get; set; }
    }
}