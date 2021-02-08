namespace Prover.Core.Models.Instruments
{
    using Newtonsoft.Json;
    using Prover.CommProtocol.Common.Items;
    using Prover.CommProtocol.Common.Models.Instrument;
    using Prover.CommProtocol.MiHoneywell;
    using Prover.Core.Models.Certificates;
    using Prover.Core.Models.Clients;
    using Prover.Core.Models.Instruments.DriveTypes;
    using Prover.Core.Settings;
    using Prover.Core.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Instrument" />
    /// </summary>
    public partial class Instrument : ProverBaseEntity
    {
        /// <summary>
        /// Gets or sets the ArchivedDateTime
        /// </summary>
        [Index]
        public DateTime? ArchivedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Certificate
        /// </summary>
        public virtual Certificate Certificate { get; set; }

        /// <summary>
        /// Gets or sets the CertificateId
        /// </summary>
        [Index]
        public Guid? CertificateId { get; set; }

        /// <summary>
        /// Gets or sets the Client
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// Gets or sets the ClientId
        /// </summary>
        [Index]
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the CommPortsPassed
        /// </summary>
        public bool? CommPortsPassed { get; set; }

        /// <summary>
        /// Gets the CompositionType
        /// </summary>
        [NotMapped]
        public EvcCorrectorType CompositionType
        {
            get
            {
                if (string.Equals(Items.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase) && string.Equals(Items.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                {
                    return EvcCorrectorType.PTZ;
                }

                if (string.Equals(Items.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                {
                    return EvcCorrectorType.P;
                }

                if (string.Equals(Items.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                {
                    return EvcCorrectorType.T;
                }

                return EvcCorrectorType.T;
            }
        }

        /// <summary>
        /// Gets or sets the EmployeeId
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the EventLogPassed
        /// </summary>
        public bool? EventLogPassed { get; set; }

        /// <summary>
        /// Gets or sets the ExportedDateTime
        /// </summary>
        [Index]
        public DateTime? ExportedDateTime { get; set; } = null;

        /// <summary>
        /// Gets the FirmwareVersion
        /// </summary>
        [NotMapped]
        public decimal FirmwareVersion => Items.GetItem(ItemCodes.SiteInfo.Firmware).NumericValue;

        /// <summary>
        /// Gets a value indicating whether HasPassed
        /// </summary>
        [NotMapped]
        public bool HasPassed
        {
            get
            {
                var verificationTestsPassed = VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;
                if (VolumeTest.DriveType is MechanicalDrive)
                {
                    return verificationTestsPassed
                        && EventLogPassed != null && EventLogPassed.Value
                        && CommPortsPassed != null && CommPortsPassed.Value;
                }

                return verificationTestsPassed;
            }
        }

        /// <summary>
        /// Gets or sets the InstrumentType
        /// </summary>
        [NotMapped]
        public override IEvcDevice InstrumentType { get; set; }

        /// <summary>
        /// Gets the InstrumentTypeString
        /// </summary>
        [NotMapped]
        public string InstrumentTypeString => InstrumentType.ToString();

        /// <summary>
        /// Gets the InventoryNumber
        /// </summary>
        [NotMapped]
        public string InventoryNumber => Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue;

        /// <summary>
        /// Gets a value indicating whether IsLivePressure
        /// </summary>
        [NotMapped]
        public bool IsLivePressure => CompositionType == EvcCorrectorType.PTZ || CompositionType == EvcCorrectorType.P;

        /// <summary>
        /// Gets a value indicating whether IsLiveSuper
        /// </summary>
        [NotMapped]
        public bool IsLiveSuper => CompositionType == EvcCorrectorType.PTZ;

        /// <summary>
        /// Gets a value indicating whether IsLiveTemperature
        /// </summary>
        [NotMapped]
        public bool IsLiveTemperature => CompositionType == EvcCorrectorType.PTZ ||
                                         CompositionType == EvcCorrectorType.T;

        /// <summary>
        /// Gets or sets the JobId
        /// </summary>
        public string JobId { get; set; }

        public virtual Instrument LinkedTest { get; set; }

        public Guid? LinkedTestId { get; set; }

        /// <summary>
        /// Gets the PulseAScaling
        /// </summary>
        [NotMapped]
        public decimal PulseAScaling => Items.GetItem(56).NumericValue;

        /// <summary>
        /// Gets the PulseASelect
        /// </summary>
        [NotMapped]
        public string PulseASelect => Items.GetItem(93).Description;

        /// <summary>
        /// Gets the PulseBScaling
        /// </summary>
        [NotMapped]
        public decimal PulseBScaling => Items.GetItem(57).NumericValue;

        /// <summary>
        /// Gets the PulseBSelect
        /// </summary>
        [NotMapped]
        public string PulseBSelect => Items.GetItem(94).Description;

        /// <summary>
        /// Gets the PulseCScaling
        /// </summary>
        [NotMapped]
        public decimal PulseCScaling => Items.GetItem(58).NumericValue;

        /// <summary>
        /// Gets the PulseCSelect
        /// </summary>
        [NotMapped]
        public string PulseCSelect => Items.GetItem(95).Description;

        /// <summary>
        /// Gets the PulseOutputTiming
        /// </summary>
        [NotMapped]
        public decimal PulseOutputTiming => Items.GetItem(115)?.NumericValue ?? 0;

        /// <summary>
        /// Gets the SiteNumber1
        /// </summary>
        [NotMapped]
        public decimal SiteNumber1 => Items.GetItem(200).NumericValue;

        /// <summary>
        /// Gets the SiteNumber2
        /// </summary>
        [NotMapped]
        public decimal SiteNumber2 => Items.GetItem(201).NumericValue;

        /// <summary>
        /// Gets or sets the TestDateTime
        /// </summary>
        public DateTime TestDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the VerificationTests
        /// </summary>
        public virtual List<VerificationTest> VerificationTests { get; set; } = new List<VerificationTest>();

        /// <summary>
        /// Gets the VolumeTest
        /// </summary>
        [NotMapped]
        public VolumeTest VolumeTest
        {
            get
            {
                var firstOrDefault = VerificationTests.FirstOrDefault(vt => vt.VolumeTest != null);
                if (firstOrDefault != null)
                {
                    return firstOrDefault.VolumeTest;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the SerialNumber
        /// </summary>
        [NotMapped]
        public int SerialNumber => (int)Items.GetItem(ItemCodes.SiteInfo.SerialNumber).NumericValue;

        /// <summary>
        /// Gets the Transducer
        /// </summary>
        [NotMapped]
        public TransducerType Transducer => (TransducerType)Items.GetItem(ItemCodes.Pressure.TransducerType).NumericValue;

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="itemValues">The itemValues<see cref="IEnumerable{ItemValue}"/></param>
        /// <param name="testSettings">The testSettings<see cref="TestSettings"/></param>
        /// <param name="client">The client<see cref="Client"/></param>
        /// <returns>The <see cref="Instrument"/></returns>
        public static Instrument Create(IEvcDevice instrumentType, IEnumerable<ItemValue> itemValues,
            TestSettings testSettings, Client client = null)
        {
            var i = new Instrument()
            {
                TestDateTime = DateTime.Now,
                CertificateId = null,
                Type = instrumentType.Id,
                InstrumentType = instrumentType,
                Items = itemValues.ToList(),

                Client = client,
                ClientId = client?.Id,
                EventLogPassed = true,
                CommPortsPassed = true
            };

            i.VerificationTests = AddVerificationTests(i, testSettings);

            return i;
        }

        public override void OnInitializing()
        {
            InstrumentType = HoneywellInstrumentTypes.GetById(Type);

            base.OnInitializing();
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $@"{ JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) }";
        }

        private static List<VerificationTest> AddVerificationTests(Instrument instrument, TestSettings testSettings)
        {
            var results = new List<VerificationTest>();

            if (instrument.InstrumentType == HoneywellInstrumentTypes.Toc)
            {
                var firstLevel = testSettings.TestPoints.Find(t => t.Level == 0);
                results.Add(VerificationTest.Create(instrument, testSettings, firstLevel));
                return results;
            }
            else
            {
                foreach (var tp in testSettings.TestPoints)
                {
                    var vt = VerificationTest.Create(instrument, testSettings, tp);
                    results.Add(vt);
                }
            }
            return results;
        }
    }

    /// <summary>
    /// Defines the <see cref="Instrument" />
    /// </summary>
    public partial class Instrument
    {
        /// <summary>
        /// The CanExport
        /// </summary>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> CanExport()
        {
            return i => i.ExportedDateTime == null && i.ArchivedDateTime == null;
        }

        /// <summary>
        /// The HasNoCertificate
        /// </summary>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> HasNoCertificate()
        {
            return i => i.CertificateId == null || i.CertificateId == Guid.Empty && i.ArchivedDateTime == null;
        }

        /// <summary>
        /// The IsArchived
        /// </summary>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> IsArchived()
        {
            return i => i.ArchivedDateTime != null;
        }

        /// <summary>
        /// The IsExported
        /// </summary>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> IsExported()
        {
            return i => i.ExportedDateTime != null;
        }

        /// <summary>
        /// The IsNotOfInstrumentType
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="string"/></param>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> IsNotOfInstrumentType(string instrumentType)
        {
            return i => i.InstrumentType.Name != instrumentType;
        }

        /// <summary>
        /// The IsOfInstrumentType
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="string"/></param>
        /// <returns>The <see cref="Predicate{Instrument}"/></returns>
        public static Predicate<Instrument> IsOfInstrumentType(string instrumentType)
        {
            return i => string.Equals(i.InstrumentType.Name, instrumentType, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(instrumentType) || string.Equals(instrumentType, "all", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The GetDateFormatted
        /// </summary>
        /// <param name="dateTime">The dateTime<see cref="DateTime"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetDateFormatted(DateTime dateTime)
        {
            var dateFormat = Items.GetItem(262).Description;
            dateFormat = dateFormat.Replace("YY", "yy").Replace("DD", "dd");
            return dateTime.ToString(dateFormat);
        }

        /// <summary>
        /// The GetDateTime
        /// </summary>
        /// <returns>The <see cref="DateTime"/></returns>
        public DateTime GetDateTime()
        {
            var dateFormat = Items.GetItem(262).Description;
            dateFormat = dateFormat.Replace("YY", "yy").Replace("DD", "dd");
            dateFormat = $"{dateFormat} HH mm ss";
            var time = Items.GetItem(203).RawValue;
            var date = Items.GetItem(204).RawValue;

            return DateTime.ParseExact($"{date} {time}", dateFormat, null);
        }

        /// <summary>
        /// The GetTimeFormatted
        /// </summary>
        /// <param name="dateTime">The dateTime<see cref="DateTime"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetTimeFormatted(DateTime dateTime)
        {
            return dateTime.ToString("HH mm ss");
        }
    }
}