namespace Prover.Core.Domain.Models.QaTestRuns.QaTestPoints
{
    public class VolumeTestPointDto
    {
        public VolumeTestPointDto(int pulseACount, int pulseBCount, decimal appliedInput, string driveTypeDiscriminator,
            decimal trueUncorrected, decimal evcStartUncorrected, decimal evcEndUncorrected, decimal trueCorrected,
            decimal evcStartCorrected, decimal evcEndCorrected)
        {
            PulseACount = pulseACount;
            PulseBCount = pulseBCount;
            AppliedInput = appliedInput;
            DriveTypeDiscriminator = driveTypeDiscriminator;
            TrueUncorrected = trueUncorrected;
            EvcStartUncorrected = evcStartUncorrected;
            EvcEndUncorrected = evcEndUncorrected;
            TrueCorrected = trueCorrected;
            EvcStartCorrected = evcStartCorrected;
            EvcEndCorrected = evcEndCorrected;
        }

        protected VolumeTestPointDto()
        {
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }
        public string DriveTypeDiscriminator { get; set; }

        //UnCorrected
        public decimal TrueUncorrected { get; set; }
        public decimal EvcStartUncorrected { get; set; }
        public decimal EvcEndUncorrected { get; set; }

        //Corrected
        public decimal TrueCorrected { get; set; }
        public decimal EvcStartCorrected { get; set; }
        public decimal EvcEndCorrected { get; set; }
    }
}