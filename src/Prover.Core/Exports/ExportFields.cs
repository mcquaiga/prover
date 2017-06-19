using System;
using System.Linq;
using Prover.Core.Shared.Enums;

namespace Prover.Core.Exports
{
    public class ExportFields
    {
        public string[] GetPropertyNames()
        {
            return GetType().GetProperties().Select(p => p.Name).ToArray();
        }

        public string VerificationType { get; set; }
        public DateTime TestedDate { get; set; }

        public string CorrectorType { get; set; }
        public string CompanyNumber { get; set; }
        public string SerialNumber { get; set; }

        public long CorrectedMultiplier { get; set; }
        public long UncorrectMultiplier { get; set; }

        public string RotaryMeterType { get; set; }

        public decimal? TemperatureHighError { get; set; }
        public decimal? TemperatureMediumError { get; set; }
        public decimal? TemperatureLowError { get; set; }

        public decimal? PressureHighError { get; set; }
        public decimal? PressureMediumError { get; set; }
        public decimal? PressureLowError { get; set; }

        public decimal? SuperHighError { get; set; }
        public decimal? SuperMediumError { get; set; }
        public decimal? SuperLowError { get; set; }

        public decimal? CorrectedVolumeError { get; set; }
        public decimal? UncorrectedVolumeError { get; set; }
        public string CorrectedMultiplierDescription { get; set; }
        public string UncorrectMultiplierDescription { get; set; }
    }
}