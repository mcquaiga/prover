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
            //var commPort = new IrDAPort();
            
            using (var client = new MiniMaxClient(commPort))
            {
                try
                {
                    client.Connect(10).Wait();

                    var sn = client.GetItemValue(62).Result;
                    System.Console.WriteLine($"Serial Number = {sn}");
                }
                catch (AggregateException ex)
                {
                    System.Console.WriteLine(ex.Flatten().Message);
                }
            }

            System.Console.ReadLine();
        }
    }
}
