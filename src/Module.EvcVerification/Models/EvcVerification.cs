namespace Module.EvcVerification.Models
{
    using Core.Domain;
    using Module.EvcVerification.Models.CorrectionTestAggregate;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="EvcVerification"/>
    /// </summary>
    public class EvcVerification : AggregateRoot
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ArchivedDateTime
        /// </summary>
        public DateTime? ArchivedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the CommPortsPassed
        /// </summary>
        public bool? CommPortsPassed { get; set; }

        /// <summary>
        /// Gets or sets the VerificationTests
        /// </summary>
        public virtual IReadOnlyCollection<CorrectionTest> CorrectionTests => _correctionTests.AsReadOnly();

        public virtual IEvcDevice EvcDevice { get; private set; }

        /// <summary>
        /// Gets or sets the EventLogPassed
        /// </summary>
        public bool? EventLogPassed { get; set; }

        /// <summary>
        /// Gets or sets the TestDateTime
        /// </summary>
        public DateTimeOffset TestDateTime { get; set; }

        /// <summary>
        /// Gets the VolumeTest
        /// </summary>
        public VolumeTest VolumeTest
        {
            get
            {
                var firstOrDefault = CorrectionTests.FirstOrDefault(vt => vt.VolumeTest != null);
                if (firstOrDefault != null)
                {
                    return firstOrDefault.VolumeTest;
                }

                return null;
            }
        }

        private readonly List<CorrectionTest> _correctionTests = new List<CorrectionTest>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="instrumentType">The instrumentType <see cref="InstrumentType"/></param>
        /// <param name="itemValues">The itemValues <see cref="IEnumerable{ItemValue}"/></param>
        /// <param name="testSettings">The testSettings <see cref="TestSettings"/></param>
        /// <param name="client">The client <see cref="Client"/></param>
        /// <returns>The <see cref="EvcVerification"/></returns>
        public static EvcVerification Create(IEvcDevice evcType, Client client = null)
        {
            var i = new EvcVerification()
            {
                TestDateTime = DateTime.Now,
                EvcDevice = evcType
            };

            return i;
        }

        public void AddCorrectionTest(CorrectionTestDefinition tp)
        {
            var test = new CorrectionTest(this, tp.Level);

            if (EvcDevice.CompositionType == EvcCorrectorType.P)
            {
                test.AddPressure(tp.PressureGaugePercent);
            }

            if (EvcDevice.CompositionType == EvcCorrectorType.T)
            {
                test.AddTemperature(tp.TemperatureGauge);
            }

            if (EvcDevice.CompositionType == EvcCorrectorType.PTZ)
            {
                test.AddPressure(tp.PressureGaugePercent);
                test.AddTemperature(tp.TemperatureGauge);
                test.AddSuperFactor(this);
            }

            if (tp.IsVolumeTest)
            {
                test.AddVolume(tp.MechanicalDriveTestLimits);

                if (EvcDevice.HasFrequency)
                {
                    test.AddFrequency();
                }
            }
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $@"{ JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) }";
        }

        #endregion Methods
    }
}

/// <summary>
/// Defines the <see cref="EvcVerification"/>
/// </summary>
//public partial class Instrument
//{
//    #region Methods
//    /// <summary>
//    /// The GetDateFormatted
//    /// </summary>
//    /// <param name="dateTime">The dateTime<see cref="DateTime"/></param>
//    /// <returns>The <see cref="string"/></returns>
//    public string GetDateFormatted(DateTime dateTime)
//    {
//        var dateFormat = Items.GetItem(262).Description;
//        dateFormat = dateFormat.Replace("YY", "yy").Replace("DD", "dd");
//        return dateTime.ToString(dateFormat);
//    }

// ///
// <summary>
// /// The GetDateTime ///
// </summary>
// ///
// <returns>The <see cref="DateTime"/></returns>
// public DateTime GetDateTime() { var dateFormat = Items.GetItem(262).Description; dateFormat =
// dateFormat.Replace("YY", "yy").Replace("DD", "dd"); dateFormat = $"{dateFormat} HH mm ss"; var
// time = Items.GetItem(203).RawValue; var date = Items.GetItem(204).RawValue;

// return DateTime.ParseExact($"{date} {time}", dateFormat, null); }

// ///
// <summary>
// /// The GetTimeFormatted ///
// </summary>
// ///
// <param name="dateTime">The dateTime <see cref="DateTime"/></param>
// ///
// <returns>The <see cref="string"/></returns>
// public string GetTimeFormatted(DateTime dateTime) { return dateTime.ToString("HH mm ss"); }

// /// <summary> /// The CanExport /// </summary> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification> CanExport() {
// return i => i.ExportedDateTime == null && i.ArchivedDateTime == null; }

// /// <summary> /// The HasNoCertificate /// </summary> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification>
// HasNoCertificate() { return i => i.CertificateId == null || i.CertificateId == Guid.Empty &&
// i.ArchivedDateTime == null; }

// /// <summary> /// The IsArchived /// </summary> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification> IsArchived() {
// return i => i.ArchivedDateTime != null; }

// /// <summary> /// The IsExported /// </summary> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification> IsExported() {
// return i => i.ExportedDateTime != null; }

// /// <summary> /// The IsNotOfInstrumentType /// </summary> /// <param name="instrumentType">The
// instrumentType<see cref="string"/></param> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification>
// IsNotOfInstrumentType(string instrumentType) { return i => i.InstrumentType.Name !=
// instrumentType; }

// /// <summary> /// The IsOfInstrumentType /// </summary> /// <param name="instrumentType">The
// instrumentType<see cref="string"/></param> /// <returns>The <see
// cref="Predicate{Instrument}"/></returns> public static Predicate<EvcVerification>
// IsOfInstrumentType(string instrumentType) { return i => string.Equals(i.InstrumentType.Name,
// instrumentType, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(instrumentType) ||
// string.Equals(instrumentType, "all", StringComparison.OrdinalIgnoreCase); }

//    #endregion
//}