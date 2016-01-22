using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    abstract class Instrument
    {
        protected SerialPort _serialPort;

        public Instrument()
        {

        }
    }
}
