using System;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.Shared.Domain;
using Prover.Shared.Enums;
using Prover.Shared.Helpers;

namespace Prover.Domain.Model.Verifications
{
    public abstract class VolumeTest : Entity
    {
        protected VolumeTest() : base((Guid) Guid.NewGuid())
        {
            AppliedInput = 0;
            PulserA = 0;
            PulserB = 0;
        }

        public double AppliedInput { get; set; }
        public double CorrectedEndReading { get; set; }

        public double CorrectedMultiplier { get; set; }
        public double CorrectedStartReading { get; set; }
        public double CorrectedTotal => CorrectedEndReading - CorrectedStartReading;
        public double DriveRate { get; set; }
        public int PulserA { get; set; }
        public int PulserB { get; set; }
        public double UncorrectedEndReading { get; set; }

        public double UncorrectedMultiplier { get; set; }
        public double UncorrectedStartReading { get; set; }
        public double UncorrectedTotal => UncorrectedEndReading - UncorrectedStartReading;

        public abstract double UncorrectedInputVolume();
    }

    public class MechanicalVolumeTest : VolumeTest
    {
        private const string Dktherms = "DecaTherms";
        private const string GigaJoules = "GigaJoules";
        private const string KiloCals = "KiloCals";
        private const string KiloWattHours = "KiloWattHours";
        private const string MegaJoules = " MegaJoules";
        private const string Therms = "Therms";
        public double EnergyEndReading { get; set; }

        public double EnergyStartReading { get; set; }
        public double EnergyTotal => EnergyEndReading - EnergyStartReading;
        public string EnergyUnits { get; set; }
        public double GasEnergyTotal { get; set; }

        public double CalculatedEnergy()
        {
            switch (EnergyUnits)
            {
                case Therms:
                    return Math.Round(GasEnergyTotal * CorrectedTotal) / 100000;
                case Dktherms:
                    return Math.Round(GasEnergyTotal * CorrectedTotal) / 1000000;
                case GigaJoules:
                    break;
            }

            return default(double);
        }

        public double? PercentError()
        {
            return CalculationHelpers.CalculatePercentError(CalculatedEnergy(), EnergyTotal);
        }

        public override double UncorrectedInputVolume()
        {
            return AppliedInput * DriveRate;
        }
    }

    public class RotaryVolumeTest : VolumeTest, IRotaryMeterItems
    {
        public double MeterDisplacement { get; set; }
        public string MeterModel { get; set; }
        public int MeterModelId { get; set; }

        public override double UncorrectedInputVolume()
        {
            return AppliedInput * MeterDisplacement;
        }
    }
}