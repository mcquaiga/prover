using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Settings;

namespace Prover.Core.Models.Instruments
{
    public enum CorrectorType
    {
        T,
        P,
        // ReSharper disable once InconsistentNaming
        PTZ
    }

    public class Instrument : ProverTable
    {
        public Instrument()
        {
        }

        public Instrument(InstrumentType instrumentType, IEnumerable<ItemValue> items, Client client = null)
        {
            TestDateTime = DateTime.Now;
            Type = instrumentType.Id;
            InstrumentType = instrumentType;
            Items = items.ToList();

            Client = client;
            ClientId = client?.Id;

            CreateVerificationTests();
        }

        public Instrument(IEnumerable<ItemValue> items, DateTime testDateTime, DateTime? archivedDateTime, InstrumentType instrumentType, Guid? clientId, Client client, string employeeId, string jobId, DateTime? exportedDateTime, bool? eventLogPassed, bool? commPortsPassed, List<VerificationTest> verificationTests)
        {
            Items = items.ToList();
            TestDateTime = testDateTime;
            ArchivedDateTime = archivedDateTime;
            InstrumentType = instrumentType;
            ClientId = clientId;
            Client = client;
            EmployeeId = employeeId;
            JobId = jobId;
            ExportedDateTime = exportedDateTime;
            EventLogPassed = eventLogPassed;
            CommPortsPassed = commPortsPassed;
            VerificationTests = verificationTests;
        }

        public Instrument(IEnumerable<ItemValue> items, DateTime testDateTime, DateTime? archivedDateTime, DateTime? exportedDateTime, InstrumentType instrumentType, string employeeId, string jobId, bool? eventLogPassed, bool? commPortsPassed, Client client, List<VerificationTest> verificationTests)
        {
            Items = items.ToList();
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
            VerificationTests.ForEach(x =>
            {
                x.Instrument = this;
                x.OnInitializing();
            });
        }

        public DateTime TestDateTime { get; set; }

        public DateTime? ArchivedDateTime { get; set; }

        public int Type { get; set; }

        [NotMapped, JsonIgnore]
        public override InstrumentType InstrumentType { get; set; }

        //public Guid? CertificateId { get; set; }

        //public virtual Certificate Certificate { get; set; }

        public Guid? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public string EmployeeId { get; set; }

        public string JobId { get; set; }

        public DateTime? ExportedDateTime { get; set; } = null;

        public bool? EventLogPassed { get; set; }

        public bool? CommPortsPassed { get; set; }

        public virtual List<VerificationTest> VerificationTests { get; set; } = new List<VerificationTest>();

        public void CreateVerificationTests(int defaultVolumeTestNumber = 0)
        {
            for (var i = 0; i < 3; i++)
            {
                var verificationTest = new VerificationTest(i, this);

                if (CompositionType == CorrectorType.P)
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(i));

                if (CompositionType == CorrectorType.T)
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));

                if (CompositionType == CorrectorType.PTZ)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(i));
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                    verificationTest.SuperFactorTest = new SuperFactorTest(verificationTest);
                }

                if (i == defaultVolumeTestNumber)
                    verificationTest.VolumeTest = new VolumeTest(verificationTest);

                VerificationTests.Add(verificationTest);
            }
        }

        public decimal GetGaugeTemp(int testNumber)
        {
            return
                SettingsManager.SettingsInstance.TemperatureGaugeDefaults.FirstOrDefault(t => t.Level == testNumber)
                    .Value;
        }

        public decimal GetGaugePressure(int testNumber)
        {
            var value =
                SettingsManager.SettingsInstance.PressureGaugeDefaults.FirstOrDefault(p => p.Level == testNumber).Value;

            if (value > 1)
                value = value / 100;

            var evcPressureRange = Items.GetItem(ItemCodes.Pressure.Range).NumericValue;
            return value * evcPressureRange;
        }

        #region NotMapped Properties

        [NotMapped, JsonIgnore]
        public int SerialNumber => (int) Items.GetItem(ItemCodes.SiteInfo.SerialNumber).NumericValue;

        [NotMapped, JsonIgnore]
        public string InventoryNumber => Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue;

        [NotMapped, JsonIgnore]
        public string InstrumentTypeString => InstrumentType.ToString();

        [NotMapped, JsonIgnore]
        public CorrectorType CompositionType
        {
            get
            {
                if (Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live"
                    && Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live")
                    return CorrectorType.PTZ;

                if (Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live")
                    return CorrectorType.P;

                if (Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live")
                    return CorrectorType.T;

                return CorrectorType.T;
            }
        }

        [NotMapped, JsonIgnore]
        public bool HasPassed
        {
            get
            {
                var verificationTestsPassed = VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;
                if (InstrumentType == CommProtocol.Common.InstrumentTypes.Instruments.MiniAt)
                    return verificationTestsPassed && EventLogPassed != null && EventLogPassed.Value &&
                           CommPortsPassed != null && CommPortsPassed.Value;

                return verificationTestsPassed;
            }
        }

        [NotMapped, JsonIgnore]
        public decimal FirmwareVersion => Items.GetItem(ItemCodes.SiteInfo.Firmware).NumericValue;

        [NotMapped, JsonIgnore]
        public decimal PulseAScaling => Items.GetItem(56).NumericValue;

        [NotMapped, JsonIgnore]
        public string PulseASelect => Items.GetItem(93).Description;

        [NotMapped, JsonIgnore]
        public decimal PulseBScaling => Items.GetItem(57).NumericValue;

        [NotMapped, JsonIgnore]
        public string PulseBSelect => Items.GetItem(94).Description;

        [NotMapped, JsonIgnore]
        public decimal SiteNumber1 => Items.GetItem(200).NumericValue;

        [NotMapped, JsonIgnore]
        public decimal SiteNumber2 => Items.GetItem(201).NumericValue;

        [NotMapped, JsonIgnore]
        public VolumeTest VolumeTest
        {
            get
            {
                var firstOrDefault = VerificationTests.FirstOrDefault(vt => vt.VolumeTest != null);
                return firstOrDefault?.VolumeTest;
            }
        }

        #endregion
    }
}