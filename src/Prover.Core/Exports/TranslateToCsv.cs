using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.Shared.Enums;

namespace Prover.Core.Exports
{
    internal static class TranslateToExport
    {
        private const int Level3Number = 2;
        private const int Level2Number = 1;
        private const int Level1Number = 0;

        public static IList<ExportFields> Translate(Certificate certificate)
        {
            var instruments = certificate.Instruments.ToList();

            return instruments
                .Select(x => Translate(certificate, x))
                .ToList();
        }

        public static ExportFields Translate(Certificate certificate, Instrument instrument)
        {
            try
            {
                var csvFormat = new ExportFields
                {
                    InstrumentType = instrument.InstrumentType.Name,
                    CompanyNumber = instrument.InventoryNumber,
                    SerialNumber = instrument.SerialNumber.ToString(),
                    VerificationType = certificate.VerificationType,
                    TestedDate = instrument.TestDateTime,

                    TemperatureLevel1Error = GetTempTestPercentError(instrument, Level1Number) ?? 0.0m,
                    TemperatureLevel2Error = GetTempTestPercentError(instrument, Level2Number) ?? 0.0m,
                    TemperatureLevel3Error = GetTempTestPercentError(instrument, Level3Number) ?? 0.0m,

                    PressureLevel1Error = GetPressureTestPercentError(instrument, Level1Number) ?? 0.0m,
                    PressureLevel2Error = GetPressureTestPercentError(instrument, Level2Number) ?? 0.0m,
                    PressureLevel3Error = GetPressureTestPercentError(instrument, Level3Number) ?? 0.0m,

                    SuperLevel1Error = GetSuperTestPercentError(instrument, Level1Number) ?? 0.0m,
                    SuperLevel2Error = GetSuperTestPercentError(instrument, Level2Number) ?? 0.0m,
                    SuperLevel3Error = GetSuperTestPercentError(instrument, Level3Number) ?? 0.0m,

                    CorrectedMultiplier = instrument.CorrectedMultiplier(),
                    CorrectedMultiplierDescription = instrument.CorrectedMultiplierDescription(),
                    UncorrectMultiplier = instrument.UnCorrectedMultiplier(),
                    UncorrectMultiplierDescription = instrument.UnCorrectedMultiplierDescription(),

                    CorrectedVolumeError = instrument.VolumeTest.CorrectedPercentError,
                    UncorrectedVolumeError = instrument.VolumeTest.UnCorrectedPercentError,
                    CorrectorType = Enum.GetName(typeof(EvcCorrectorType), instrument.CompositionType),
                    RotaryMeterType = (instrument.VolumeTest.DriveType as RotaryDrive)?.Meter.MeterTypeDescription,

                    Items = new Dictionary<int, string>(
                        instrument.Items.ToDictionary(
                            k => k.Metadata.Number,
                            v => v.Metadata.ItemDescriptions != null && v.Metadata.ItemDescriptions.Any() ? v.Description : v.RawValue)
                    )
                };
                return csvFormat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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