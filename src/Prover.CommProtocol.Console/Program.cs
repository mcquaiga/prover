using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.Instruments;

namespace Prover.CommProtocol.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commPort = new SerialPort("COM3", 38400);
            var instrument = HoneywellInstruments.CreateInstrument(commPort, HoneywellInstrumentType.MiniMax);

            System.Console.ReadLine();
        }
    }
}
