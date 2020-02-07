using System;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public class EmptyDInOutBoard : IDInOutBoard
    {
        public decimal PulseTiming { get; set; }

        public void Dispose()
        {
            return;
        }

        public int ReadInput()
        {
            return 0;
        }

        public void StartMotor()
        {
        }

        public void StopMotor()
        {
        }
    }
}