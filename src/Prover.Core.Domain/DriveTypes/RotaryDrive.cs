using System;
using Prover.CommProtocol.Common;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.Extensions;

namespace Prover.Domain.DriveTypes
{
    public class RotaryDrive : IDriveType
    {
        public RotaryDrive(MeterIndexInfo meterIndexInfo, decimal evcMeterDisplacement, int uncorrectedMultiplier)
        {
            UncorrectedMultiplier = uncorrectedMultiplier;
            Meter = new MeterTest(meterIndexInfo, evcMeterDisplacement);
        }

        public int UncorrectedMultiplier { get; }

        public MeterTest Meter { get; }

        public string Discriminator => "Rotary";

        public bool HasPassed => Meter.HasPassed;

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return Meter.MeterDisplacement * appliedInput;
        }

        public int MaxUncorrectedPulses
        {
            get
            {
                if (UncorrectedMultiplier == 10)
                    return Meter.MeterIndexInfo.UnCorPulsesX10;

                if (UncorrectedMultiplier == 100)
                    return Meter.MeterIndexInfo.UnCorPulsesX100;

                return 10; //Low standard number if we can't find anything    
            }
        }
    }

    public class MeterTest
    {
        public MeterTest(MeterIndexInfo meterIndex, decimal evcMeterDisplacement)
        {
            MeterIndexInfo = meterIndex; // MeterIndexInfo.Get((int) _instrument.Items.GetItem(432).NumericValue);
            EvcMeterDisplacement = evcMeterDisplacement;
        }

        public bool HasPassed => PercentError.IsBetween(1);

        public decimal MeterDisplacement => MeterIndexInfo?.MeterDisplacement ?? 0;

        public decimal EvcMeterDisplacement { get; set; }

        public decimal? PercentError 
            => MeterDisplacement != 0 ? Math.Round((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement * 100, 2) : default(decimal?);

        public MeterIndexInfo MeterIndexInfo { get; }

        public string MeterTypeDescription => MeterIndexInfo.Description;

        public string MeterType => MeterIndexInfo.Description;

        public int MeterTypeId => (int) MeterIndexInfo.Id;
    }
}