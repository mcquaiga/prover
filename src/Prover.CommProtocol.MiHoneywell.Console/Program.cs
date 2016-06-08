using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commPort = new SerialPortV2("COM11", 38400);
 
            using (var client = new HoneywellClient(commPort, InstrumentType.MiniMax))
            {
                try
                {
                    client.Connect(10).Wait();

                    while (true) // Loop indefinitely
                    {
                        try
                        {
                            System.Console.WriteLine("Enter item number to read or exit:");
                            var line = System.Console.ReadLine();
                            if (line == "exit") // Check string
                            {
                                break;
                            }

                            int item;
                            if (int.TryParse(line, out item))
                            {

                                    var itemValue = client.GetItemValue(item).Result;
                                    System.Console.WriteLine($"{itemValue}");
                           
                            }
                            else if (line.Contains(","))
                            {
                                var items = line.Split(',').Select(int.Parse);
                                var itemValues = client.GetItemValues(items).Result;

                                itemValues.ForEach(System.Console.WriteLine);
                            }
                            else
                            {
                                System.Console.WriteLine("Didn't understand that.");
                            }
                        }
                        catch (AggregateException ex)
                        {
                            System.Console.WriteLine(ex.Flatten().InnerException.Message);
                        }
                    }
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
