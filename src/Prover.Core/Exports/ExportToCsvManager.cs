using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Exports
{
    public class CsvExportFormat
    {
        public string InstrumentNumber { get; set; }
        public string MakerNumber { get; set; }
        public long CorrectedIndex { get; set; }
        public long UncorrectedIndex { get; set; }
        public string LotNumber { get; set; } = string.Empty;
        public string RepairClass { get; set; }

        public decimal GiHighTemperatureError { get; set; }
        public decimal GiMediumTemperatureError { get; set; }
        public decimal GiLowTemperatureError { get; set; }

        public decimal GiHighPressureError { get; set; }
        public decimal GiMediumPressureError { get; set; }
        public decimal GiLowPressureError { get; set; }

        public decimal GiHighSuperError { get; set; }
    }

    public class ExportToCsvManager : IExportTestRun
    {
        private readonly ICertificateStore _certificateStore;

        public ExportToCsvManager(ICertificateStore certificateStore)
        {
            _certificateStore = certificateStore;
        }
        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument> { instrumentForExport };
            return await Export(instrumentList);
        }

        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            var instrs = instrumentsForExport.ToList();

            var certId = instrs.First().CertificateId.Value;
            var cert = _certificateStore.GetCertificate(certId);

            var file = File.OpenWrite($"{AppDomain.CurrentDomain.BaseDirectory}\\{cert.Number}.csv");

            foreach (var instrument in instrs)
            {
                var csvLine = 
            }
        }
    }

    internal static class TranslateToCsv
    {
        public string Translate(Certificate certificate, Instrument instrument)
        {
            var isNewVerification = certificate.VerificationType == "New";

            var csvFormat = new CsvExportFormat()
            {
                InstrumentNumber = instrument.InventoryNumber,
                MakerNumber = instrument.SerialNumber.ToString(),
                CorrectedIndex = !isNewVerification ? (long)instrument.Items.GetItem(90).NumericValue : 0,
                UncorrectedIndex = !isNewVerification ? (long)instrument.Items.GetItem(92).NumericValue : 0,
                LotNumber = string.Empty,
                RepairClass = !isNewVerification ? "1" : string.Empty,
                GiHighTemperatureError = 
            }
        }
    }
}
