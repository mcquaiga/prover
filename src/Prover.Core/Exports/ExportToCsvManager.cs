using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Extensions;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using System.Collections.Generic;

namespace Prover.Core.Exports
{
    public enum VerificationTypeEnum
    {
        New,
        Reverified
    }

    public interface IFileWriter
    {
        void Write();
    }

    public class ExportToCsvManager : IExportCertificate
    {
        private static readonly string CertificatesExportDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\Certificates\\";

        private readonly IProverStore<Certificate> _certificateStore;
        private readonly IProverStore<Instrument> _instrumentStore;

        public ExportToCsvManager(IProverStore<Certificate> certificateStore, IProverStore<Instrument> instrumentStore)
        {
            _certificateStore = certificateStore;
            _instrumentStore = instrumentStore;
        }

        public async Task<string[]> Export(Client client, long fromCertificateNumber, long toCertificateNumber)
        {
            var certificates = _certificateStore.Query()
                .Where(c => c.ClientId == client.Id && (c.Number >= fromCertificateNumber && c.Number <= toCertificateNumber))
                .OrderBy(c => c.Number)
                .ToList();

            var csvFiles = new List<string>();
            foreach (var cert in certificates)
            {
                csvFiles.Add(await Export(cert));
            }

            return csvFiles.ToArray();
        }

        public async Task<string> Export(Certificate certificate)
        {
            return await Task.Run(async () =>
            {
                var instrs = certificate
                    .Instruments
                    .Select(i => _instrumentStore.Get(i.Id))
                    .ToList();

                var certificateId = instrs.FirstOrDefault()?.CertificateId;
                if (certificateId == null)
                    throw new NullReferenceException("Certificate could not be found.");

                var client = certificate.Client;
                var fileName = GetFilePath(certificate, client);

                try
                {
                    var csvFormat = FindCsvTemplate(certificate, instrs, client);                    

                    var exportInstruments = TranslateToExport.Translate(certificate).ToList();

                    await CsvWriter.Write(fileName, csvFormat, exportInstruments);

                    return fileName;
                }
                catch
                {
                    throw;
                }
            });
        }

        private static string FindCsvTemplate(Certificate certificate, System.Collections.Generic.List<Instrument> instrs, Client client)
        {
            var csvFormat = client.CsvTemplates.FirstOrDefault(c => 
                        c.VerificationTypeString == certificate.VerificationType && 
                        c.CorrectorType == instrs.First().CompositionType && 
                        c.InstrumentType == instrs.First().InstrumentType)?.CsvTemplate;

            if (csvFormat == null)
                throw new Exception($"Could not find a CSV format that matches the following criteria for Certificate #{certificate.Number}. {Environment.NewLine}" +
                    $"  Verification Type: {certificate.VerificationType} {Environment.NewLine}" +
                    $"  Corrector Type: {instrs.First().CompositionType} {Environment.NewLine}" +
                    $"  Instrument: {instrs.First().InstrumentType.Name} {Environment.NewLine}");
            return csvFormat;
        }

        private static string GetFilePath(Certificate certificate, Client client)
        {
            var fileName = $"certificate_{certificate.Number}.csv";

            var filePath = $"{CertificatesExportDirectory}{client.Name}";
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            return $"{filePath}\\{fileName}";
        }
    }
}


//public CsvExportFormat(string instrumentNumber, string makerNumber, long correctedIndex, long uncorrectedIndex,
//string repairClass, decimal giHighTemperatureError, decimal giMediumTemperatureError,
//decimal giLowTemperatureError, decimal giHighPressureError, decimal giMediumPressureError,
//decimal giLowPressureError, decimal giHighSuperError, decimal giMediumSuperError, decimal giLowSuperError,
//decimal giCorrectedVolumeError, string testedDate, InspectionTypEnum inspectionType, string repairDate)
//{
//InspectionType = inspectionType.ToString();

//InstrumentNumber = instrumentNumber;
//MakerNumber = makerNumber;

//CorrectedIndex = inspectionType == InspectionTypEnum.New ? "0" : ;
//UncorrectedIndex = uncorrectedIndex;
//RepairClass = inspectionType == InspectionTypEnum.New ? string.Empty : "1";
//GiHighTemperatureError = giHighTemperatureError;
//GiMediumTemperatureError = giMediumTemperatureError;
//GiLowTemperatureError = giLowTemperatureError;
//GiHighPressureError = giHighPressureError;
//GiMediumPressureError = giMediumPressureError;
//GiLowPressureError = giLowPressureError;
//GiHighSuperError = giHighSuperError;
//GiMediumSuperError = giMediumSuperError;
//GiLowSuperError = giLowSuperError;
//GiCorrectedVolumeError = giCorrectedVolumeError;
//TestedDate = testedDate;

//RepairDate = repairDate;
//}

//public override string ToString()
//{
//    return $"{InstrumentNumber}," +
//           $"{MakerNumber}," +
//           $"{CorrectedIndex}," +
//           $"{UncorrectedIndex}," +
//           $"{LotNumber}," +
//           $"{RepairClass}," +

//           //New
//           $"{GiHighTemperatureError}," +
//           $"{GiMediumTemperatureError}," +
//           $"{GiLowTemperatureError}," +
//           $"{GiHighPressureError}," +
//           $"{GiMediumPressureError}," +
//           $"{GiLowPressureError}," +
//           $"{GiHighSuperError}," +
//           $"{GiMediumSuperError}," +
//           $"{GiLowSuperError}," +
//           $"{GiCorrectedVolumeError}," +

//           // Reverified only information
//           $"{HighTemperatureError}," +
//           $"{MediumTemperatureError}," +
//           $"{LowTemperatureError}," +
//           $"{HighPressureError}," +
//           $"{MediumPressureError}," +
//           $"{LowPressureError}," +
//           $"{HighSuperError}," +
//           $"{MediumSuperError}," +
//           $"{LowSuperError}," +
//           $"{CorrectedVolumeError}," +

//           $"{TestedDate}," +
//           $"{OperatorNumber}," +
//           $"{InspectionType}," +
//           $"{RepairDate}," +
//           $"{PlanAndOption}," +
//           $"{ProverNumber}";

//}

//public class CsvExportFormat
//{    
//    public string InstrumentNumber { get; set; }
//    public string MakerNumber { get; set; }
//    public long CorrectedIndex { get; set; }
//    public long UncorrectedIndex { get; set; }
//    public string LotNumber { get; set; }
//    public string RepairClass { get; set; }

//    // Verification Type = new && reverification
//    public decimal GiHighTemperatureError { get; set; }

//    public decimal GiMediumTemperatureError { get; set; }
//    public decimal GiLowTemperatureError { get; set; }

//    public decimal GiHighPressureError { get; set; }
//    public decimal GiMediumPressureError { get; set; }
//    public decimal GiLowPressureError { get; set; }

//    public decimal GiHighSuperError { get; set; }
//    public decimal GiMediumSuperError { get; set; }
//    public decimal GiLowSuperError { get; set; }

//    public decimal GiCorrectedVolumeError { get; set; }

//    // Verification Type = blank for new, same as the Gi's reverified
//    public decimal? HighTemperatureError { get; set; }
//    public decimal? MediumTemperatureError { get; set; }
//    public decimal? LowTemperatureError { get; set; }

//    public decimal? HighPressureError { get; set; }
//    public decimal? MediumPressureError { get; set; }
//    public decimal? LowPressureError { get; set; }

//    public decimal? HighSuperError { get; set; }
//    public decimal? MediumSuperError { get; set; }
//    public decimal? LowSuperError { get; set; }

//    public decimal? CorrectedVolumeError { get; set; }

//    //Test Information
//    public string TestedDate { get; set; }
//    public int OperatorNumber { get; set; }
//    public string InspectionType { get; set; }
//    public string RepairDate { get; set; }
//    public string PlanAndOption { get; set; }
//    public long ProverNumber { get; set; }

//}