using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        private InstrumentType _instrumentType;
        

        [NotMapped] public IEnumerable<ItemMetadata> ItemDetails;
        
        private Instrument()
        {
        }

        public Instrument(InstrumentType instrumentType, IEnumerable<ItemMetadata> items,
            Dictionary<int, string> itemValues)
        {
            TestDateTime = DateTime.Now;
            CertificateId = null;
            Type = instrumentType;
            ItemDetails = items;
            ItemValues = itemValues;
        }

        public DateTime TestDateTime { get; set; }

        public InstrumentType Type
        {
            get { return _instrumentType; }
            set
            {
                if (ItemDetails == null || !ItemDetails.Any())
                {
                    ItemDetails = ItemHelpers.LoadItems(value);
                }
                _instrumentType = value;
            }
        }

        public Guid? CertificateId { get; set; }
        public virtual Certificate Certificate { get; set; }

        public DateTime? ExportedDateTime { get; set; } = null;

        public virtual List<VerificationTest> VerificationTests { get; set; } = new List<VerificationTest>();

        #region NotMapped Properties

        [NotMapped]
        public int SerialNumber => (int) ItemDetails.GetItem(SERIAL_NUMBER).GetNumericValue(ItemValues);

        [NotMapped]
        public string TypeString => Type.ToString();

        [NotMapped]
        public CorrectorType CompositionType
        {
            get
            {
                if (ItemDetails.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live"
                    && ItemDetails.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.PTZ;

                if (ItemDetails.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.P;

                if (ItemDetails.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.T;

                return CorrectorType.T;
            }
        }

        [NotMapped]
        public bool HasPassed => VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;

        [NotMapped]
        public decimal FirmwareVersion
        {
            get { return ItemDetails.GetItem(122).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public decimal PulseAScaling
        {
            get { return ItemDetails.GetItem(56).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public string PulseASelect
        {
            get { return ItemDetails.GetItem(93).GetDescriptionValue(ItemValues); }
        }

        [NotMapped]
        public decimal PulseBScaling
        {
            get { return ItemDetails.GetItem(57).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public string PulseBSelect
        {
            get { return ItemDetails.GetItem(94).GetDescriptionValue(ItemValues); }
        }

        [NotMapped]
        public decimal SiteNumber1
        {
            get { return ItemDetails.GetItem(200).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public decimal SiteNumber2
        {
            get { return ItemDetails.GetItem(201).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public VolumeTest VolumeTest => VerificationTests.FirstOrDefault(vt => vt.VolumeTest != null).VolumeTest;

        #endregion
    }
}