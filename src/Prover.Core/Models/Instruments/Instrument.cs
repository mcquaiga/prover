using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Extensions;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.Settings;
using Prover.Core.Shared.Enums;
using Prover.Core.Shared.Extensions;

namespace Prover.Core.Models.Instruments
{
    public enum CorrectorType
    {
        T,
        P,
        // ReSharper disable once InconsistentNaming
        PTZ
    }

    public partial class Instrument : ProverTable
    {
        public Instrument()
        {
        }

        public Instrument(InstrumentType instrumentType, IEnumerable<ItemValue> items, Client client = null)
        {
            TestDateTime = DateTime.Now;
            CertificateId = null;
            Type = instrumentType.Id;
            InstrumentType = instrumentType;
            Items = items.ToList();

            Client = client;
            ClientId = client?.Id;

            CreateVerificationTests();
        }

        public DateTime TestDateTime { get; set; }

        [Index]
        public DateTime? ArchivedDateTime { get; set; }

        public int Type { get; set; }

        [NotMapped]
        public override InstrumentType InstrumentType { get; set; }

        [Index]
        public Guid? CertificateId { get; set; }

        public virtual Certificate Certificate { get; set; }

        [Index]
        public Guid? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public string EmployeeId { get; set; }

        public string JobId { get; set; }

        [Index]
        public DateTime? ExportedDateTime { get; set; } = null;

        public bool? EventLogPassed { get; set; }

        public bool? CommPortsPassed { get; set; }

        public virtual List<VerificationTest> VerificationTests { get; set; } = new List<VerificationTest>();

        public DateTime GetDateTime()
        {
            var dateFormat = Items.GetItem(262).Description;
            dateFormat = dateFormat.Replace("YY", "yy").Replace("DD", "dd");
            dateFormat = $"{dateFormat} HH mm ss";

            var time = Items.GetItem(203).RawValue;
            var date = Items.GetItem(204).RawValue;

            return DateTime.ParseExact($"{date} {time}", dateFormat, null);
        }

        public string GetDateFormatted(DateTime dateTime)
        {
            var dateFormat = Items.GetItem(262).Description;
            dateFormat = dateFormat.Replace("YY", "yy").Replace("DD", "dd");
            return dateTime.ToString(dateFormat);
        }

        public string GetTimeFormatted(DateTime dateTime)
        {
            return dateTime.ToString("HH mm ss");
        }

        public void CreateVerificationTests(int defaultVolumeTestNumber = 0)
        {
            for (var i = 0; i < 3; i++)
            {
                var verificationTest = new VerificationTest(i, this);

                if (CompositionType == EvcCorrectorType.P)
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(i));

                if (CompositionType == EvcCorrectorType.T)
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));

                if (CompositionType == EvcCorrectorType.PTZ)
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
            var result = SettingsManager.SettingsInstance.TestSettings.TemperatureGaugeDefaults.FirstOrDefault(t => t.Level == testNumber)?.Value;
            return result.HasValue ? TemperatureTest.ConvertTo(result.Value, "F", this.TemperatureUnits()) : 0m;
        }

        public decimal GetGaugePressure(int testNumber)
        {
            var value = SettingsManager.SettingsInstance.TestSettings.PressureGaugeDefaults
                .FirstOrDefault(p => p.Level == testNumber)?.Value;
            
            if (value > 1)
                value = value / 100;

            var evcPressureRange = Items.GetItem(ItemCodes.Pressure.Range).NumericValue;
            return value * evcPressureRange ?? 0.0m;
        }

        #region NotMapped Properties

        [NotMapped]
        public int SerialNumber => (int) Items.GetItem(ItemCodes.SiteInfo.SerialNumber).NumericValue;

        [NotMapped]
        public string InventoryNumber => Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue;

        [NotMapped]
        public string InstrumentTypeString => InstrumentType.ToString();

        [NotMapped]
        public EvcCorrectorType CompositionType
        {
            get
            {
                if (Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live"
                    && Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live")
                    return EvcCorrectorType.PTZ;

                if (Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live")
                    return EvcCorrectorType.P;

                if (Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live")
                    return EvcCorrectorType.T;

                return EvcCorrectorType.T;
            }
        }

        [NotMapped]
        public bool IsLiveTemperature => CompositionType == EvcCorrectorType.PTZ ||
                                         CompositionType == EvcCorrectorType.T;

        [NotMapped]
        public bool IsLivePressure => CompositionType == EvcCorrectorType.PTZ || CompositionType == EvcCorrectorType.P;

        [NotMapped]
        public bool IsLiveSuper => CompositionType == EvcCorrectorType.PTZ;

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                var verificationTestsPassed = VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;
                if (VolumeTest.DriveType is MechanicalDrive)
                    return verificationTestsPassed 
                        && EventLogPassed != null && EventLogPassed.Value 
                        && CommPortsPassed != null && CommPortsPassed.Value;

                return verificationTestsPassed;
            }
        }

        [NotMapped]
        public decimal FirmwareVersion => Items.GetItem(ItemCodes.SiteInfo.Firmware).NumericValue;

        [NotMapped]
        public decimal PulseAScaling => Items.GetItem(56).NumericValue;

        [NotMapped]
        public string PulseASelect => Items.GetItem(93).Description;

        [NotMapped]
        public decimal PulseBScaling => Items.GetItem(57).NumericValue;

        [NotMapped]
        public string PulseBSelect => Items.GetItem(94).Description;

        [NotMapped]
        public decimal PulseCScaling => Items.GetItem(58).NumericValue;

        [NotMapped]
        public string PulseCSelect => Items.GetItem(95).Description;

        [NotMapped]
        public decimal SiteNumber1 => Items.GetItem(200).NumericValue;

        [NotMapped]
        public decimal SiteNumber2 => Items.GetItem(201).NumericValue;

        [NotMapped]
        public VolumeTest VolumeTest
        {
            get
            {
                var firstOrDefault = VerificationTests.FirstOrDefault(vt => vt.VolumeTest != null);
                if (firstOrDefault != null)
                    return firstOrDefault.VolumeTest;
                return null;
            }
        }

        [NotMapped]
        public TransducerType Transducer
            => (TransducerType) Items.GetItem(ItemCodes.Pressure.TransducerType).NumericValue;

        #endregion

        public override string ToString()
        {
            return $@"{
                    JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
                        {ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
                }";
        }
    }

    public partial class Instrument
    {
        public static Predicate<Instrument> IsExported()
        {
            return i => i.ExportedDateTime != null;
        }

        public static Predicate<Instrument> IsArchived()
        {
            return i => i.ArchivedDateTime != null;
        }

        public static Predicate<Instrument> CanExport()
        {
            return i => i.ExportedDateTime == null && i.ArchivedDateTime == null;
        }

        public static Predicate<Instrument> HasNoCertificate()
        {
            return i => i.CertificateId == null || i.CertificateId == Guid.Empty && i.ArchivedDateTime == null;
        }

        public static Predicate<Instrument> IsOfInstrumentType(string instrumentType)
        {
            return i => i.InstrumentType.Name.ToLower() == instrumentType.ToLower() || string.IsNullOrEmpty(instrumentType) || instrumentType.ToLower() == "all";
        }

        public static Predicate<Instrument> IsNotOfInstrumentType(string instrumentType)
        {
            return i => i.InstrumentType.Name != instrumentType;
        }
    }
}