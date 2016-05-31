using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Prover.Core.Communication;
using Prover.Core.Models.Certificates;
using Prover.SerialProtocol;
using System.Collections.ObjectModel;

namespace Prover.Core.Models.Instruments
{
    public enum InstrumentType
    {
        MiniMax = 4,
        MiniAt = 3,
        Ec300 = 7
    }

    public enum CorrectorType
    {
        T,
        P,
        // ReSharper disable once InconsistentNaming
        PTZ
    }

    public class Instrument : ProverTable
    {
        private int FIXED_PRESSURE_FACTOR = 109;
        private int FIXED_SUPER_FACTOR = 110;
        private int FIXED_TEMP_FACTOR = 111;
        private int SERIAL_NUMBER = 62;
        private InstrumentType _instrumentType;
        private InstrumentType instrumentType;

        [NotMapped]
        public InstrumentItems Items;

        private Instrument()
        {
        }

        public Instrument(InstrumentType instrumentType, InstrumentItems items, Dictionary<int, string> itemValues)
        {
            TestDateTime = DateTime.Now;
            CertificateId = null;
            Type = instrumentType;
            Items = items;
            ItemValues = itemValues;
        }

        public DateTime TestDateTime { get; set; }
        public InstrumentType Type
        {
            get
            {
                return _instrumentType;
            }
            set
            {
                if (Items == null || !Items.Items.Any())
                {
                    Items = new InstrumentItems(value);
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
        public int SerialNumber
        {
            get { return (int)Items.GetItem(SERIAL_NUMBER).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public string TypeString
        {
            get { return Type.ToString(); }
        }

        [NotMapped] 
        public CorrectorType CompositionType
        {
            get
            {
                if (Items.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live"
                  && Items.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.PTZ;

                if (Items.GetItem(FIXED_PRESSURE_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.P;

                if (Items.GetItem(FIXED_TEMP_FACTOR).GetDescriptionValue(ItemValues).ToLower() == "live")
                    return CorrectorType.T;

                return CorrectorType.T;
            }
        }

        [NotMapped]
        public bool HasPassed => VerificationTests.FirstOrDefault(x => x.HasPassed == false) == null;

        [NotMapped]
        public decimal FirmwareVersion
        {
            get
            {
                return Items.GetItem(122).GetNumericValue(ItemValues);
            }
        }

        [NotMapped]
        public decimal PulseAScaling
        {
            get { return Items.GetItem(56).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public string PulseASelect
        {
            get { return Items.GetItem(93).GetDescriptionValue(ItemValues); }
        }

        [NotMapped]
        public decimal PulseBScaling
        {
            get { return Items.GetItem(57).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public string PulseBSelect
        {
            get { return Items.GetItem(94).GetDescriptionValue(ItemValues); }
        }

        [NotMapped]
        public decimal SiteNumber1
        {
            get { return Items.GetItem(200).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public decimal SiteNumber2
        {
            get { return Items.GetItem(201).GetNumericValue(ItemValues); }
        }

        [NotMapped]
        public VolumeTest VolumeTest => VerificationTests.FirstOrDefault(vt => vt.VolumeTest != null).VolumeTest;
        #endregion
    }
}
