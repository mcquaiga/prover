using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.Instruments;
using Prover.Domain.Models.VerificationTests;

namespace Prover.CommProtocol.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commPort = new SerialPort("COM3", 38400);
            var instrument = HoneywellInstrumentFactory.Create(commPort, HoneywellInstrumentType.MiniMax).Result;

            var test = TestRunFactory.Create(instrument);

            var testDto = TestRunFactory.Create(test);

            var test2 = TestRunFactory.Create(testDto);
            System.Console.ReadLine();
        }
    }
}
