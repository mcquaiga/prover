using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniMax;
using Prover.Domain.Instrument;
using Prover.Domain.Verification.TestRun;

namespace Prover.CommProtocol.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commPort = new SerialPort("COM3", 38400);

            var factory = new MiniMaxFactory(commPort);
            var instrument = factory.Create().Result;
            var itemData = JsonConvert.SerializeObject(instrument.ItemData);
            System.Console.ReadLine();
        }
    }
}