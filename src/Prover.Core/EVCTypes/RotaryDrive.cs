using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.EVCTypes
{
    public class RotaryDrive : IDriveType
    {
        private Instrument _instrument;
        private MeterTest _meter;

        public RotaryDrive(Instrument instrument)
        {
            _instrument = instrument;
            _meter = new MeterTest(_instrument);
        }

        public string Discriminator
        {
            get
            {
                return "Rotary";
            }
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return (_meter.MeterDisplacement * appliedInput);
        }

        public int MaxUnCorrected()
        {
            if (_instrument.UnCorrectedMultiplier() == 10)
                return _meter.MeterIndex.UnCorPulsesX10;

            if (_instrument.UnCorrectedMultiplier() == 100)
                return _meter.MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }
    }

    public class MeterTest
    {
        private Instrument _instrument;

        public MeterTest(Instrument instrument)
        {
            _instrument = instrument;
            MeterIndex = MeterIndexInfo.Get((int)_instrument.Items.GetItem(432).GetNumericValue());
        }

        public bool MeterDisplacementHasPassed
        {
            get { return (MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD)); }
        }

        public decimal MeterDisplacement
        {
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        }

        public decimal? EvcMeterDisplacement
        {
            get
            {
                return _instrument.Items.GetItem(439).GetNumericValue();
            }
        }

        public decimal MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                {
                    return Math.Round((decimal)(((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement) * 100), 2);
                }
                return 0;
            }
        }

        public MeterIndexInfo MeterIndex { get; private set; }

        public string MeterTypeDescription
        {
            get { return MeterIndex.Description; }
        }

        public string MeterType
        {
            get { return _instrument.Items.GetItem(432).GetDescriptionValue(); }
        }


        public int MeterTypeId
        {
            get { return (int)_instrument.Items.GetItem(432).GetNumericValue(); }
        }
    }
}
