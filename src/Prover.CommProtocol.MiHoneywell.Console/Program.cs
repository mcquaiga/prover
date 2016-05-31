using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commPort = new SerialCommPort("COM11", 9600);
            var client = new MiniMaxClient(commPort);

            client.Connect().Wait();

            System.Console.ReadLine();

            client.Disconnect().Wait();
        }
    }
}
