using System;
using Prover.CommProtocol.Common.Items;
using Prover.Core.EVCTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public class RotaryDrive : IDriveType
    {
        public Instrument Instrument { get; set; }
        public MeterTest Meter { get; set; }

        public RotaryDrive(Instrument instrument)
        {
            Instrument = instrument;
            Meter = new MeterTest(Instrument);
        }

        public string Discriminator => "Rotary";

        public bool HasPassed => Meter.MeterDisplacementHasPassed;

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return (Meter.MeterDisplacement * appliedInput);
        }

        public int MaxUncorrectedPulses()
        {
            if (Instrument.UnCorrectedMultiplier() == 10)
                return Meter.MeterIndex.UnCorPulsesX10;

            if (Instrument.UnCorrectedMultiplier() == 100)
                return Meter.MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }
    }

    public class MeterTest
    {
        private readonly Instrument _instrument;

        public MeterTest(Instrument instrument)
        {
            _instrument = instrument;
            MeterIndex = MeterIndexInfo.Get((int)_instrument.Items.GetItem(432).NumericValue);
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
                return _instrument.Items.GetItem(439).NumericValue;
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
            get { return _instrument.Items.GetItem(432).Description; }
        }


        public int MeterTypeId
        {
            get { return (int)_instrument.Items.GetItem(432).NumericValue; }
        }
    }
}
