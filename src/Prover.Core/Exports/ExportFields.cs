using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.Core.Exports
{
    public class ExportFields
    {
        public string[] GetPropertyDescriptions()
        {
            return GetType()
                .GetProperties()
                .Select(p => p.GetCustomAttributes(true)
                                 .Where(c => c is CsvTemplateAttribute)
                                 .Select(d => (d as CsvTemplateAttribute).Name)
                                 .FirstOrDefault()
                                 ?? p.Name)
                .ToArray();
        }

        public string[] GetPropertyNames()
        {
            return GetType()
                .GetProperties()
                .Select(p => p.Name)                                
                .ToArray();
        }

        [CsvTemplate("Verification Type")]
        public string VerificationType { get; set; }

        [CsvTemplate("Tested Date")]
        public DateTime TestedDate { get; set; }

        [CsvTemplate("Instrument Type")]
        public string InstrumentType { get; set; }

        [CsvTemplate("Corrector Type")]
        public string CorrectorType { get; set; }

        [CsvTemplate("Company Number")]
        public string CompanyNumber { get; set; }

        [CsvTemplate("Serial Number")]
        public string SerialNumber { get; set; }

        [CsvTemplate("Corrected Multiplier")]
        public decimal CorrectedMultiplier { get; set; }

        [CsvTemplate("Uncorrected Multiplier")]
        public decimal UncorrectMultiplier { get; set; }

        [CsvTemplate("Rotary Meter Type")]
        public string RotaryMeterType { get; set; }

        [CsvTemplate("Temperature Level 3 Error")]
        public decimal? TemperatureLevel3Error { get; set; }
        [CsvTemplate("Temperature Level 2 Error")]
        public decimal? TemperatureLevel2Error { get; set; }
        [CsvTemplate("Temperature Level 1 Error")]
        public decimal? TemperatureLevel1Error { get; set; }

        [CsvTemplate("Pressure Level 1 Error")]
        public decimal? PressureLevel1Error { get; set; }
        [CsvTemplate("Pressure Level 2 Error")]
        public decimal? PressureLevel2Error { get; set; }
        [CsvTemplate("Pressure Level 3 Error")]
        public decimal? PressureLevel3Error { get; set; }

        [CsvTemplate("Super Level 1 Error")]
        public decimal? SuperLevel1Error { get; set; }
        [CsvTemplate("Super Level 2 Error")]
        public decimal? SuperLevel2Error { get; set; }
        [CsvTemplate("Super Level 3 Error")]
        public decimal? SuperLevel3Error { get; set; }

        [CsvTemplate("Corrected Volume Error")]
        public decimal? CorrectedVolumeError { get; set; }

        [CsvTemplate("Uncorrected Volume Error")]
        public decimal? UncorrectedVolumeError { get; set; }

        [CsvTemplate("Corrected Multiplier Description")]
        public string CorrectedMultiplierDescription { get; set; }

        [CsvTemplate("Uncorrected Multiplier Description")]
        public string UncorrectMultiplierDescription { get; set; }

        [CsvTemplate("Item")]
        public Dictionary<int, string> Items { get; set; }
    }
}