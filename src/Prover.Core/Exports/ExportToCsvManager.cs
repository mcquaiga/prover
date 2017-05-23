using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Exports
{
    public enum InspectionTypEnum
    {
        New,
        Reverified
    }

    public class CsvExportFormat
    {    
        public string InstrumentNumber { get; set; }
        public string MakerNumber { get; set; }
        public long CorrectedIndex { get; set; }
        public long UncorrectedIndex { get; set; }
        public string LotNumber { get; set; }
        public string RepairClass { get; set; }

        // Verification Type = new && reverification
        public decimal GiHighTemperatureError { get; set; }

        public decimal GiMediumTemperatureError { get; set; }
        public decimal GiLowTemperatureError { get; set; }

        public decimal GiHighPressureError { get; set; }
        public decimal GiMediumPressureError { get; set; }
        public decimal GiLowPressureError { get; set; }

        public decimal GiHighSuperError { get; set; }
        public decimal GiMediumSuperError { get; set; }
        public decimal GiLowSuperError { get; set; }

        public decimal GiCorrectedVolumeError { get; set; }

        // Verification Type = blank for new, same as the Gi's reverified
        public decimal? HighTemperatureError { get; set; }
        public decimal? MediumTemperatureError { get; set; }
        public decimal? LowTemperatureError { get; set; }

        public decimal? HighPressureError { get; set; }
        public decimal? MediumPressureError { get; set; }
        public decimal? LowPressureError { get; set; }

        public decimal? HighSuperError { get; set; }
        public decimal? MediumSuperError { get; set; }
        public decimal? LowSuperError { get; set; }

        public decimal? CorrectedVolumeError { get; set; }

        //Test Information
        public string TestedDate { get; set; }
        public int OperatorNumber { get; set; }
        public string InspectionType { get; set; }
        public string RepairDate { get; set; }
        public string PlanAndOption { get; set; }
        public long ProverNumber { get; set; }
        
    }

    public interface IExportCertificate : IExportTestRun
    {
        
    }

    public class ExportToCsvManager : IExportCertificate
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
            
            var certificateId = instrs.FirstOrDefault()?.CertificateId;
            if (certificateId == null) return false;

            var cert = await _certificateStore.GetCertificate(certificateId.Value);

            using (var file = File.OpenWrite($"{AppDomain.CurrentDomain.BaseDirectory}\\Certificates\\certificate_{cert.Number}.csv"))
            {
                using (var fs = new StreamWriter(file))
                {
                    using (var csv = new CsvWriter(fs))
                    {
                        csv.WriteHeader(typeof(CsvExportFormat));
                        csv.WriteRecords(TranslateToCsv.Translate(cert, instrs));
                    }
                }
            }
            
            return true;
        }
    }

    internal static class TranslateToCsv
    {
        private const int HighTestNumber = 2;
        private const int MidTestNumber = 1;
        private const int LowTestNumber = 0;

        private const string NewVerificationType = "New";

        public static IList<CsvExportFormat> Translate(Certificate certificate, IEnumerable<Instrument> instruments)
        {
            return instruments.ToList()
                .Select(x => Translate(certificate, x))
                .ToList();
        }

        public static CsvExportFormat Translate(Certificate certificate, Instrument instrument)
        {
            var isNewVerification = certificate.VerificationType == NewVerificationType;

            var csvFormat = new CsvExportFormat()
            {
                InstrumentNumber = instrument.InventoryNumber,
                MakerNumber = instrument.SerialNumber.ToString(),
                LotNumber = string.Empty,
                InspectionType = "N",
                TestedDate = instrument.TestDateTime.ToString("yyyyMMdd"),
                RepairDate = string.Empty,
                RepairClass = string.Empty,
                CorrectedIndex = 0,
                UncorrectedIndex = 0,

                OperatorNumber = 74,
                PlanAndOption = "S3",
                ProverNumber = 74,

                GiHighTemperatureError = GetTempTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                GiMediumTemperatureError = GetTempTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                GiLowTemperatureError = GetTempTestPercentError(instrument, LowTestNumber) ?? 0.0m,

                GiHighPressureError = GetPressureTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                GiMediumPressureError = GetPressureTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                GiLowPressureError = GetPressureTestPercentError(instrument, LowTestNumber) ?? 0.0m,

                GiHighSuperError = GetSuperTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                GiMediumSuperError = GetSuperTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                GiLowSuperError = GetSuperTestPercentError(instrument, LowTestNumber) ?? 0.0m
            };

            // Reverified add extra error columns
            if (!isNewVerification)
            {
                csvFormat.InspectionType = "R";
                csvFormat.RepairClass = "1";
                csvFormat.RepairDate = instrument.TestDateTime.ToString("yyyyMMdd");

                csvFormat.CorrectedIndex = (long) instrument.Items.GetItem(90).NumericValue;
                csvFormat.UncorrectedIndex = (long) instrument.Items.GetItem(92).NumericValue;

                csvFormat.HighTemperatureError = csvFormat.GiHighTemperatureError;
                csvFormat.MediumTemperatureError = csvFormat.GiMediumTemperatureError;
                csvFormat.LowTemperatureError = csvFormat.GiLowTemperatureError;

                csvFormat.HighPressureError = csvFormat.GiHighPressureError;
                csvFormat.MediumPressureError = csvFormat.GiMediumPressureError;
                csvFormat.LowPressureError = csvFormat.GiLowPressureError;

                csvFormat.HighSuperError = csvFormat.GiHighSuperError;
                csvFormat.MediumSuperError = csvFormat.GiMediumSuperError;
                csvFormat.LowSuperError = csvFormat.GiLowSuperError;
            }

            return csvFormat;
        }

        private static decimal? GetTempTestPercentError(Instrument instrument, int testLevel)
        {
            var test = instrument.VerificationTests.FirstOrDefault(vt => vt.TestNumber == testLevel)?.TemperatureTest;

            return test?.PercentError;
        }

        private static decimal? GetPressureTestPercentError(Instrument instrument, int testLevel)
        {
            var test = instrument.VerificationTests.FirstOrDefault(vt => vt.TestNumber == testLevel)?.PressureTest;

            return test?.PercentError;
        }

        private static decimal? GetSuperTestPercentError(Instrument instrument, int testLevel)
        {
            var test = instrument.VerificationTests.FirstOrDefault(vt => vt.TestNumber == testLevel)?.SuperFactorTest;

            return test?.PercentError;
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
