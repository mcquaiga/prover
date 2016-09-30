using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public class EmptyDInOutBoard : IDInOutBoard
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int ReadInput()
        {
            return 0;
        }

        public void StartMotor()
        {
            return;
        }

        public void StopMotor()
        {
            return;
        }
    }
}
