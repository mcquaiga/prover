using System.Collections.Generic;

namespace Prover.Domain.Models.Instruments.Items
{
    public interface IPressureItems
    {
        int Range { get; set; }
        string TransducerType { get; set; }
        decimal Base { get; set; }
        decimal GasPressure { get; set; }
        decimal AtmPressure { get; set; }
        decimal Factor { get; set; }
        decimal UnsqrFactor { get; set; }

        Dictionary<string, string> ItemData { get; }
    }
}