using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Shared.Enums;

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

        [CsvTemplateAttribute("Verification Type")]
        public string VerificationType { get; set; }

        [CsvTemplateAttribute("Tested Date")]
        public DateTime TestedDate { get; set; }

        [CsvTemplateAttribute("Instrument Type")]
        public string InstrumentType { get; set; }

        [CsvTemplateAttribute("Corrector Type")]
        public string CorrectorType { get; set; }

        [CsvTemplateAttribute("Company Number")]
        public string CompanyNumber { get; set; }

        [CsvTemplateAttribute("Serial Number")]
        public string SerialNumber { get; set; }

        [CsvTemplateAttribute("Corrected Multiplier")]
        public long CorrectedMultiplier { get; set; }

        [CsvTemplateAttribute("Uncorrected Multiplier")]
        public long UncorrectMultiplier { get; set; }

        [CsvTemplateAttribute("Rotary Meter Type")]
        public string RotaryMeterType { get; set; }

        [CsvTemplateAttribute("Temperature Level 3 Error")]
        public decimal? TemperatureLevel3Error { get; set; }
        [CsvTemplateAttribute("Temperature Level 2 Error")]
        public decimal? TemperatureLevel2Error { get; set; }
        [CsvTemplateAttribute("Temperature Level 1 Error")]
        public decimal? TemperatureLevel1Error { get; set; }

        [CsvTemplateAttribute("Pressure Level 1 Error")]
        public decimal? PressureLevel1Error { get; set; }
        [CsvTemplateAttribute("Pressure Level 2 Error")]
        public decimal? PressureLevel2Error { get; set; }
        [CsvTemplateAttribute("Pressure Level 3 Error")]
        public decimal? PressureLevel3Error { get; set; }

        [CsvTemplateAttribute("Super Level 1 Error")]
        public decimal? SuperLevel1Error { get; set; }
        [CsvTemplateAttribute("Super Level 2 Error")]
        public decimal? SuperLevel2Error { get; set; }
        [CsvTemplateAttribute("Super Level 3 Error")]
        public decimal? SuperLevel3Error { get; set; }

        [CsvTemplateAttribute("Corrected Volume Error")]
        public decimal? CorrectedVolumeError { get; set; }

        [CsvTemplateAttribute("Uncorrected Volume Error")]
        public decimal? UncorrectedVolumeError { get; set; }

        [CsvTemplateAttribute("Corrected Multiplier Description")]
        public string CorrectedMultiplierDescription { get; set; }

        [CsvTemplateAttribute("Uncorrected Multiplier Description")]
        public string UncorrectMultiplierDescription { get; set; }

        [CsvTemplateAttribute("Item")]
        public Dictionary<int, string> Items { get; set; }
    }
}