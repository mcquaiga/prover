namespace Prover.CommProtocol.Common.Models.Instrument.Items
{
    public interface IPressureItems : IItemsGroup
    {
        double AtmosphericPressure { get; set; }
        double Base { get; set; }
        double Factor { get; set; }
        double GasPressure { get; set; }

        int Range { get; set; }
        PressureTransducerType TransducerType { get; set; }
        PressureUnits Units { get; set; }
        double UnsqrFactor { get; set; }
    }
}