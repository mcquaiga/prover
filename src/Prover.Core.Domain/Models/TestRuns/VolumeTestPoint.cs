using Prover.Domain.DriveTypes;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class VolumeTestPoint
    {
        private decimal _pressureFactor;
        private decimal _temperatureFactor;
        private decimal _superFactor;

        public VolumeTestPoint(EvcCorrectorType correctorType)
        {
            CorrectorType = correctorType;

            TemperatureFactor = 1;
            PressureFactor = 1;
            SuperFactor = 1;

            AppliedInput = 0;
            CorrectedStart = 0;
            CorrectedEnd = 0;
            UncorrectedStart = 0;
            UncorrectedEnd = 0;
            PulseACount = 0;
            PulseBCount = 0;
            PulseCCount = 0;


        }

        public VolumeTestPoint(EvcCorrectorType correctorType, 
            decimal appliedInput, 
            decimal correctedStart, 
            decimal correctedEnd, 
            decimal uncorrectedStart, 
            decimal uncorrectedEnd, 
            int pulseACount, 
            int pulseBCount, 
            int pulseCCount, 
            decimal temperatureFactor, 
            decimal pressureFactor, 
            decimal superFactor)
        {
            CorrectorType = correctorType;
            
            TemperatureFactor = temperatureFactor;
            PressureFactor = pressureFactor;
            SuperFactor = superFactor;

            AppliedInput = appliedInput;
            CorrectedStart = correctedStart;
            CorrectedEnd = correctedEnd;
            UncorrectedStart = uncorrectedStart;
            UncorrectedEnd = uncorrectedEnd;
            PulseACount = pulseACount;
            PulseBCount = pulseBCount;
            PulseCCount = pulseCCount;
        }

        public EvcCorrectorType CorrectorType { get; protected set; }
        public decimal AppliedInput { get; set; }

        public decimal CorrectedStart { get; set; }
        public decimal CorrectedEnd { get; set; }

        public decimal UncorrectedStart { get; set; }
        public decimal UncorrectedEnd { get; set; }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public int PulseCCount { get; set; }

        //PTZ Factors
        public decimal TemperatureFactor
        {
            get
            {
                return CorrectorType == EvcCorrectorType.T || CorrectorType == EvcCorrectorType.PTZ
                    ? _temperatureFactor
                    : 1.0m;
            }
            set { _temperatureFactor = value; }
        }

        public decimal PressureFactor
        {
            get
            {
                return CorrectorType == EvcCorrectorType.P || CorrectorType == EvcCorrectorType.PTZ
                    ? _pressureFactor
                    : 1.0m;
            }
            set { _pressureFactor = value; }
        }

        public decimal SuperFactor
        {
            get { return CorrectorType == EvcCorrectorType.PTZ ? _superFactor : 1.0m; }
            set { _superFactor = value; }
        }

        public virtual IDriveType DriveType { get; protected set; }

        public virtual decimal CorrectedTotal => CorrectedEnd - CorrectedStart;

        public virtual decimal? CorrectedPercentError
            => CorrectedCalculated != 0 ? (CorrectedTotal - CorrectedCalculated) / CorrectedCalculated * 100 : default(decimal?);

        public virtual decimal CorrectedCalculated
            => TemperatureFactor * PressureFactor * SuperFactor * DriveType.UnCorrectedInputVolume(AppliedInput).Value;

        public virtual decimal UncorrectedTotal
            => UncorrectedEnd - UncorrectedStart;

        public virtual decimal? UncorrectedPercentError
            => UncorrectedCalculated != 0 ? (UncorrectedTotal - UncorrectedCalculated) / UncorrectedCalculated * 100 : default(decimal?);

        public virtual decimal UncorrectedCalculated => DriveType.UnCorrectedInputVolume(AppliedInput).Value;

        public virtual bool Passed
            => CorrectedPassed && UncorrectedPassed;

        public virtual bool CorrectedPassed
            => CorrectedPercentError < 1 && CorrectedPercentError > -1;

        public virtual bool UncorrectedPassed
            => UncorrectedPercentError < 1 && UncorrectedPercentError > -1;
    }
}