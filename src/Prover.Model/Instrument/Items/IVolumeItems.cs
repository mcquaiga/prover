using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface IVolumeItems : IItemsGroup
    {
        double CorrectedMultiplier { get; set; }
        double CorrectedReading { get; set; }
        string CorrectedUnits { get; set; }
        double DriveRate { get; set; }
        string DriveRateDescription { get; set; }
        DriveTypeDescripter DriveType { get; set; }

        double Energy { get; set; }
        double EnergyGasValue { get; set; }
        string EnergyUnits { get; set; }

        double MeterDisplacement { get; set; }
        string MeterModel { get; set; }
        int MeterModelId { get; set; }

        double UncorrectedMultiplier { get; set; }
        double UncorrectedReading { get; set; }
        string UncorrectedUnits { get; set; }
    }

    public interface IRotaryMeterItems : IItemsGroup
    {
        double MeterDisplacement { get; set; }
        string MeterModel { get; set; }
        int MeterModelId { get; set; }
    }

    public interface IEnergyItems : IItemsGroup
    {
        double EnergyGasValue { get; set; }
        double EnergyReading { get; set; }
        string EnergyUnits { get; set; }
    }
}