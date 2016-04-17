using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core
{
    public enum PressureUnits
    {
        PSIG = 0,
        PSIA = 1,
        kPa = 2,
        mPa = 3,
        BAR = 4,
        mBAR = 5,
        KGcm2 = 6,
        inWC = 7,
        inHG = 8,
        mmHG = 9
    }

    public enum TransducerType
    {
        Gauge = 0,
        Absolute = 1
    }
}
