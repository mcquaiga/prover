using System;
using System.Linq;
using System.Threading;
using Akka.Util.Internal;
using Prover.CommProtocol.Common.IO;

namespace Prover.CommProtocol.MiHoneywell.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var commPort = new SerialPort("COM3", 9600);
            var instrument = new Instruments.MiniMaxInstrument();
            using (var client = new HoneywellClient(commPort, instrument))
            {
                try
                {
                    client.Connect(CancellationToken.None).Wait();

                    var download = client.GetItemValues(instrument.ItemDefinitions);
                    var itemValues = download.Result;

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

                            string item;
                            if (!line.Contains(","))
                            {
                                //var itemValue = client.GetItemValue(item).Result;
                                System.Console.WriteLine($"{instrument.GetItemValue(line, itemValues)}");
                            }
                            else if (line.Contains(","))
                            {
                                var items = line.Split(',').Select(int.Parse);
                                //var itemValues = client.GetItemValues(items).Result;

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