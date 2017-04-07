using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Prover.CommProtocol.Common.IO;
using Prover.Domain.Instrument;

namespace Prover.CommProtocol.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commPort = new SerialPort("COM3", 38400);

            //var comm = Assembly.LoadFrom($@"{dir}\\Prover.CommProtocol.MiHoneywell.dll");

            //var type = typeof(IInstrumentFactory);
            //var types = comm.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();

            //var test = TestRunFactory.Create(instrument);

            //var testDto = TestRunFactory.Create(test);

            //var test2 = TestRunFactory.Create(testDto);
            System.Console.ReadLine();
        }
    }
}