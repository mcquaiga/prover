namespace Devices.Core.Interfaces.Items
{
    public interface IEnergyItems : IItemsGroup
    {
        #region Public Properties

        double EnergyGasValue { get; }

        double EnergyReading { get; }

        string EnergyUnits { get; }

        #endregion Public Properties
    }

    public interface IRotaryMeterItems : IItemsGroup
    {
        #region Public Properties

        double MeterDisplacement { get; }

        string MeterModel { get; }

        int MeterModelId { get; }

        #endregion Public Properties
    }

    public interface IVolumeItems : IItemsGroup
    {
        #region Public Properties

        double CorrectedMultiplier { get; }

        double CorrectedReading { get; }

        string CorrectedUnits { get; }

        double DriveRate { get; }

        string DriveRateDescription { get; }

        IDriveType DriveType { get; }

        double Energy { get; }

        double EnergyGasValue { get; }

        string EnergyUnits { get; }

        double MeterDisplacement { get; }

        string MeterModel { get; }

        int MeterModelId { get; }

        double UncorrectedMultiplier { get; }

        double UncorrectedReading { get; }

        string UncorrectedUnits { get; }

        #endregion Public Properties
    }
}