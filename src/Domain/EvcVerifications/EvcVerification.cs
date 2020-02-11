using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Domain.EvcVerifications.CorrectionTests;
using Domain.EvcVerifications.DriveTypes;
using Newtonsoft.Json;
using Shared.Domain;

namespace Domain.EvcVerifications
{
    public static class EvcVerificationExtensions
    {
        public static VerificationTestPoint GetTestPointWithVolume(this EvcVerificationTest evcVerification)
        {
            return evcVerification.Tests
                .Where(t => t.GetType() == typeof(VerificationTestPoint))
                .Select(t => (VerificationTestPoint) t)
                .FirstOrDefault(ct => ct.GetTest<CorrectedVolumeTestRun>() != null);
        }

        public static bool HasVolume(this VerificationTestPoint point)
        {
            return point.Tests.Any(t => t.GetType() == typeof(CorrectedVolumeTestRun));
        }

        public static void UpdateValues(this VerificationTestPoint vtp, IEnumerable<ItemValue> beforeValues, IEnumerable<ItemValue> afterValues, decimal appliedInput)
        {

        }

        public static T GetCorrectionTest<T>(this VerificationTestPoint vtp, CorrectionFactorTestType correctionFactorType)
            where T : CorrectionTest
        {
            return vtp.GetTest<T>(t => t.TestType == correctionFactorType);
            
            //var tc = vtp.GetTest<CorrectionTest>().GetValue();

            //return (T)tc.GetValue();
        }

        public static CorrectionTest GetCorrectionTest(this VerificationTestPoint vtp, CorrectionFactorTestType correctionFactorType)
        {
            return vtp.GetTest<CorrectionTest>(t => t.TestType == correctionFactorType);
        }
    }


    /// <summary>
    ///     Defines the <see cref="EvcVerificationTest" />
    /// </summary>
    public class EvcVerificationTest : AggregateJoin<IVerificationTest>
    {
        protected EvcVerificationTest()
        {
        }

        internal EvcVerificationTest(DeviceInstance device)
        {
            Device = device;
            DeviceType = device.DeviceType;
            TestDateTime = DateTimeOffset.UtcNow;
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets the ArchivedDateTime
        /// </summary>
        public DateTimeOffset? ArchivedDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the CommPortsPassed
        /// </summary>
        //public bool? CommPortsPassed { get; set; }

        public virtual DeviceInstance Device { get; protected set; }

        public virtual DeviceType DeviceType { get; protected set; }

        public virtual IVolumeInputType DriveType { get; set; }

        /// <summary>
        ///     Gets or sets the EventLogPassed
        /// </summary>
        //public bool? EventLogPassed { get; set; }

        /// <summary>
        ///     Gets or sets the TestDateTime
        /// </summary>
        public DateTimeOffset TestDateTime { get; set; }

        #endregion

        #region Public Methods

        #endregion
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

//public abstract class BaseVerificationEntity : BaseEntity
//{
//    public virtual IEvcDevice EvcDevice { get; }

//    //public override void OnInitializing()
//    //{
//    //    if (EvcDeviceType == null)
//    //        throw new NullReferenceException(nameof(EvcDeviceType));
//    //    if (string.IsNullOrEmpty(_evcData))
//    //        throw new NullReferenceException(nameof(_evcData));

//    //    var itemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_evcData);
//    //    //Items = ItemHelpers.LoadItems(EvcDeviceType, itemValues).ToList();
//    //}
//}