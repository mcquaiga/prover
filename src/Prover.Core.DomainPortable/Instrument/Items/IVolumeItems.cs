namespace Prover.Core.DomainPortable.Instrument.Items
{
    public interface IVolumeItems
    {
        decimal CorrectedMultiplier { get; }
        decimal CorrectedReading { get; }
        string CorrectedUnits { get; }
        decimal DriveRate { get; }
        string DriveRateDescription { get; }

        DriveTypeDescripter DriveType { get; }
        decimal Energy { get; }
        decimal EnergyGasValue { get; }

        string EnergyUnits { get; }
        decimal MeterDisplacement { get; }
        string MeterModel { get; }

        int MeterModelId { get; }
        decimal UncorrectedMultiplier { get; }
        decimal UncorrectedReading { get; }
        string UncorrectedUnits { get; }
    }
}