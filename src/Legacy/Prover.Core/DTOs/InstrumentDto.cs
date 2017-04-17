using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DTOs
{
    public static class DtoMappers
    {
        public static InstrumentDto ToDto(this Instrument instrument)
        {
            return new InstrumentDto(
                instrument.Items,
                instrument.TestDateTime,
                instrument.ArchivedDateTime,
                instrument.ExportedDateTime,
                instrument.InstrumentType,
                instrument.EmployeeId,
                instrument.JobId,
                instrument.EventLogPassed,
                instrument.CommPortsPassed,
                instrument.Client,
                instrument.VerificationTests.ToDto().ToList()
                );
        }

        public static Instrument ToObject(this InstrumentDto instrument)
        {
            return new Instrument(
                instrument.Items,
                instrument.TestDateTime,
                instrument.ArchivedDateTime,
                instrument.ExportedDateTime,
                instrument.InstrumentType,
                instrument.EmployeeId,
                instrument.JobId,
                instrument.EventLogPassed,
                instrument.CommPortsPassed,
                instrument.Client,
                instrument.VerificationTests.ToObject().ToList()
                );
        }

        public static IEnumerable<VerificationTestDto> ToDto(this IEnumerable<VerificationTest> verificationTests)
        {
            return verificationTests.Select(verificationTest => verificationTest.ToDto()).ToList();
        }

        public static IEnumerable<VerificationTest> ToObject(this IEnumerable<VerificationTestDto> verificationTests)
        {
            return verificationTests.Select(verificationTest => verificationTest.ToObject()).ToList();
        }

        public static VerificationTestDto ToDto(this VerificationTest verificationTest)
        {
            return new VerificationTestDto(
                verificationTest.TestNumber,
                verificationTest.PressureTest?.ToDto(),
                verificationTest.TemperatureTest?.ToDto(),
                verificationTest.VolumeTest?.ToDto());
        }

        public static VerificationTest ToObject(this VerificationTestDto verificationTest)
        {
            return new VerificationTest(
                verificationTest.TestNumber,
                verificationTest.PressureTest?.ToObject(),
                verificationTest.TemperatureTest?.ToObject(),
                verificationTest.VolumeTest?.ToObject());
        }


        public static VolumeTestDto ToDto(this VolumeTest volume)
        {
            return new VolumeTestDto(
                volume.Items,
                volume.AfterTestItems,
                volume.PulseACount,
                volume.PulseBCount,
                volume.AppliedInput,
                volume.DriveTypeDiscriminator);
        }

        public static VolumeTest ToObject(this VolumeTestDto volume)
        {
            return new VolumeTest(
                volume.BeforeTestItems,
                volume.AfterTestItems,
                volume.PulseACount,
                volume.PulseBCount,
                volume.AppliedInput,
                volume.DriveTypeDiscriminator);
        }

        public static TemperatureTestDto ToDto(this TemperatureTest temperatureTest)
        {
            return new TemperatureTestDto(
                temperatureTest.Items,
                temperatureTest.Gauge);
        }

        public static TemperatureTest ToObject(this TemperatureTestDto temperatureTest)
        {
            return new TemperatureTest(
                temperatureTest.Items,
                temperatureTest.Gauge);
        }

        public static PressureTestDto ToDto(this PressureTest pressure)
        {
            return new PressureTestDto(
                pressure.Items,
                pressure.GasPressure,
                pressure.GasGauge,
                pressure.AtmosphericGauge);
        }

        public static PressureTest ToObject(this PressureTestDto pressure)
        {
            return new PressureTest(
                pressure.Items,                
                pressure.GasGauge,
                pressure.AtmosphericGauge);
        }
    }

    public class InstrumentDto
    {       
        public IEnumerable<ItemValue> Items { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; } = null;

        public InstrumentType InstrumentType { get; set; }
        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }

        public Client Client { get; set; }
        public List<VerificationTestDto> VerificationTests { get; set; }

        public InstrumentDto(IEnumerable<ItemValue> items, DateTime testDateTime, DateTime? archivedDateTime, DateTime? exportedDateTime, InstrumentType instrumentType, string employeeId, string jobId, bool? eventLogPassed, bool? commPortsPassed, Client client, List<VerificationTestDto> verificationTests)
        {
            Items = items;
            TestDateTime = testDateTime;
            ArchivedDateTime = archivedDateTime;
            ExportedDateTime = exportedDateTime;
            InstrumentType = instrumentType;
            EmployeeId = employeeId;
            JobId = jobId;
            EventLogPassed = eventLogPassed;
            CommPortsPassed = commPortsPassed;
            Client = client;
            VerificationTests = verificationTests;
        }
    }

    public class VerificationTestDto
    {
        public int TestNumber { get; set; }
        public PressureTestDto PressureTest { get; set; }
        public TemperatureTestDto TemperatureTest { get; set; }
        public VolumeTestDto VolumeTest { get; set; }

        public VerificationTestDto(int testNumber, PressureTestDto pressureTest, TemperatureTestDto temperatureTest, VolumeTestDto volumeTest)
        {
            TestNumber = testNumber;
            PressureTest = pressureTest;
            TemperatureTest = temperatureTest;
            VolumeTest = volumeTest;
        }
    }

    public class VolumeTestDto
    {
        public string DriveTypeDiscriminator { get; set; }
        public IEnumerable<ItemValue> BeforeTestItems { get; set; }
        public IEnumerable<ItemValue> AfterTestItems { get; set; }
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }

        public VolumeTestDto(IEnumerable<ItemValue> beforeTestItems, IEnumerable<ItemValue> afterTestItems, int pulseACount, int pulseBCount, decimal appliedInput, string driveTypeDiscriminator)
        {
            BeforeTestItems = beforeTestItems;
            AfterTestItems = afterTestItems;
            PulseACount = pulseACount;
            PulseBCount = pulseBCount;
            AppliedInput = appliedInput;
            DriveTypeDiscriminator = driveTypeDiscriminator;
        }
    }

    public class TemperatureTestDto
    {      
        public IEnumerable<ItemValue> Items { get; set; }
        public double Gauge { get; set; }
        public TemperatureTestDto(IEnumerable<ItemValue> items, double gauge)
        {
            Items = items;
            Gauge = gauge;
        }
    }

    public class PressureTestDto
    {
        public IEnumerable<ItemValue> Items { get; set; }
        public decimal? GasPressure { get; set; }        
        public decimal? GasGauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }

        public PressureTestDto(IEnumerable<ItemValue> items, decimal? gasPressure, decimal? gasGauge, decimal? atmosphericGauge)
        {
            Items = items;
            GasPressure = gasPressure;
            GasGauge = gasGauge;
            AtmosphericGauge = atmosphericGauge;
        }
    }
}
