using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Exports
{
    internal static class TranslateToEnbridgeCsv
    {
        private const int HighTestNumber = 2;
        private const int MidTestNumber = 1;
        private const int LowTestNumber = 0;

        private const string NewVerificationType = "New";

        public static IList<ExportFields> Translate(Certificate certificate, IEnumerable<Instrument> instruments)
        {
            return instruments.ToList()
                .Select(x => Translate(certificate, x))
                .ToList();
        }

        public static ExportFields Translate(Certificate certificate, Instrument instrument)
        {
            var isNewVerification = certificate.VerificationType == NewVerificationType;

            var csvFormat = new ExportFields()
            {
                CompanyNumber = instrument.InventoryNumber,
                SerialNumber = instrument.SerialNumber.ToString(),
                //InspectionType = certificate.VerificationType == NewVerificationType ,
                TestedDate = instrument.TestDateTime,
              
                HighTemperatureError = GetTempTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                MediumTemperatureError = GetTempTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                LowTemperatureError = GetTempTestPercentError(instrument, LowTestNumber) ?? 0.0m,

                HighPressureError = GetPressureTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                MediumPressureError = GetPressureTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                LowPressureError = GetPressureTestPercentError(instrument, LowTestNumber) ?? 0.0m,

                HighSuperError = GetSuperTestPercentError(instrument, HighTestNumber) ?? 0.0m,
                MediumSuperError = GetSuperTestPercentError(instrument, MidTestNumber) ?? 0.0m,
                LowSuperError = GetSuperTestPercentError(instrument, LowTestNumber) ?? 0.0m
            };
     
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
