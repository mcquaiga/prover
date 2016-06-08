using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Models.Certificates;

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
        public Instrument(InstrumentType instrumentType, IEnumerable<ItemValue> items)
        {
            TestDateTime = DateTime.Now;
            CertificateId = null;
            Type = instrumentType;
            InstrumentType = instrumentType;
            Items = items;
        }

        public DateTime TestDateTime { get; set; }
        public InstrumentType Type { get; }

        [NotMapped]
        public override InstrumentType InstrumentType { get; }
        public Guid? CertificateId { get; set; }
        public virtual Certificate Certificate { get; set; }

        public DateTime? ExportedDateTime { get; set; } = null;

        public virtual List<VerificationTest> VerificationTests { get; set; } = new List<VerificationTest>();

        #region NotMapped Properties

        [NotMapped]
        public int SerialNumber => (int) Items.GetItem(ItemCodes.SiteInfo.SerialNumber).NumericValue;

        [NotMapped]
        public string InstrumentTypeString => InstrumentType.ToString();

        [NotMapped]
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

        [NotMapped]
        public bool HasPassed => VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;

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

        #endregion
    }
}