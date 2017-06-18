using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Exports
{
    public enum VerificationTypEnum
    {
        New,
        Reverified
    }

    public class ExportFields
    {
        public string[] GetPropertyNames()
        {
            return GetType().GetProperties().Select(p => p.Name).ToArray();
        }

        public VerificationTypEnum VerificationType { get; set; }
        public DateTime TestedDate { get; set; }

        public CorrectorType CorrectorType { get; set; }
        public string CompanyNumber { get; set; }
        public string SerialNumber { get; set; }

        public long CorrectedMultiplier { get; set; }
        public long UncorrectMultiplier{ get; set; }        
       
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
    }

    public interface IExportCertificate
    {
        Task<bool> Export(Certificate certificate);
    }

    public class ExportToCsvManager : IExportCertificate
    {
        private readonly IFileWriter _fileWriter;

        public ExportToCsvManager(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public async Task<bool> Export(Certificate certificate)
        {
            return await Task.Run(async () =>
            {
                var instrs = certificate.Instruments.ToList();

                var client = certificate.Client;
                var fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Certificates\\{client.Name}\\certificate_{certificate.Number}.csv";
                var csvFormat = client.CsvTemplates
                    .FirstOrDefault(c => 
                        c.VerificationType.ToString() == certificate.VerificationType && c.CorrectorType == instrs.First().CompositionType)?
                    .CsvTemplate;

                var certificateId = instrs.FirstOrDefault()?.CertificateId;
                if (certificateId == null) return false;
                
                await CsvWriter.Write(fileName, csvFormat, certificate.Instruments.ToList());
                
                return true;
            });
        }
    }

    public interface IFileWriter
    {
        void Write();
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