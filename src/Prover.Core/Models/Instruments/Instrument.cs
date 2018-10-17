using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Certificates;
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

    public partial class Instrument : ProverTable
    {
        public Instrument()
        {
        }

        public Instrument(InstrumentType instrumentType, IEnumerable<ItemValue> items)
        {
            TestDateTime = DateTime.Now;
            CertificateId = null;
            Type = instrumentType.Id;
            InstrumentType = instrumentType;
            Items = items;

            CreateVerificationTests();
        }

        public DateTime TestDateTime { get; set; }

        public DateTime? ArchivedDateTime { get; set; }

        public int Type { get; set; }

        [NotMapped]
        public override InstrumentType InstrumentType { get; set; }

        public Guid? CertificateId { get; set; }

        public virtual Certificate Certificate { get; set; }

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
                {
                    verificationTest.VolumeTest = new VolumeTest(verificationTest);
                    if (InstrumentType.Name == "TOC")
                    {
                        verificationTest.FrequencyTest = new FrequencyTest(verificationTest);
                    }
                }

                VerificationTests.Add(verificationTest);
            }
        }

        public Guid? ParentTestId {get; set;}
        public virtual Instrument ParentTest {get; set;}

        public virtual ICollection<Instrument> ChildTests { get; set; }

        public decimal GetGaugeTemp(int testNumber)
        {
            return SettingsManager.SettingsInstance.TemperatureGaugeDefaults.FirstOrDefault(t => t.Level == testNumber).Value;
        }

        public decimal GetGaugePressure(int testNumber)
        {
            var value = SettingsManager.SettingsInstance.PressureGaugeDefaults.FirstOrDefault(p => p.Level == testNumber).Value;

            if (value > 1)
                value = value/100;

            var evcPressureRange = Items.GetItem(ItemCodes.Pressure.Range).NumericValue;
            return value*evcPressureRange;
        }

        #region NotMapped Properties

        [NotMapped]
        public int SerialNumber => (int) Items.GetItem(ItemCodes.SiteInfo.SerialNumber).NumericValue;

        [NotMapped]
        public string InventoryNumber => Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue;

        [NotMapped]
        public string InstrumentTypeString => InstrumentType.ToString();

        [NotMapped]
        public CorrectorType CompositionType
        {
            get
            {
                if ((Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live")
                    && (Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live"))
                    return CorrectorType.PTZ;

                if (Items.GetItem(ItemCodes.Pressure.FixedFactor).Description.ToLower() == "live")
                    return CorrectorType.P;

                if (Items.GetItem(ItemCodes.Temperature.FixedFactor).Description.ToLower() == "live")
                    return CorrectorType.T;

                return CorrectorType.T;
            }
        }

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                var verificationTestsPassed = VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;
                if (InstrumentType == CommProtocol.MiHoneywell.Instruments.MiniAt)
                {
                    return verificationTestsPassed && 
                        (EventLogPassed != null && EventLogPassed.Value) && (CommPortsPassed != null && CommPortsPassed.Value);
                }

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
        public decimal PulseOutputTiming => Items.GetItem(115).NumericValue;

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