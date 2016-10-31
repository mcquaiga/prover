using System;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;

namespace Prover.CommProtocol.MiHoneywell.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ////var commPort = new SerialPort("COM11", 38400);
            //var commPort = new IrDAPort();

            //using (var client = new HoneywellClient(commPort, InstrumentType.))
            //{
            //    try
            //    {
            //        client.Connect(10).Wait();

            //        while (true) // Loop indefinitely
            //        {
            //            try
            //            {
            //                System.Console.WriteLine("Enter item number to read or exit:");
            //                var line = System.Console.ReadLine();
            //                if (line == "exit") // Check string
            //                {
            //                    break;
            //                }

            //                int item;
            //                if (int.TryParse(line, out item))
            //                {
            //                    var itemValue = client.GetItemValue(item).Result;
            //                    System.Console.WriteLine($"{itemValue}");
            //                }
            //                else if (line.Contains(","))
            //                {
            //                    var items = line.Split(',').Select(int.Parse);
            //                    var itemValues = client.GetItemValues(items).Result;

            //                    itemValues.ForEach(System.Console.WriteLine);
            //                }
            //                else
            //                {
            //                    System.Console.WriteLine("Didn't understand that.");
            //                }
            //            }
            //            catch (AggregateException ex)
            //            {
            //                System.Console.WriteLine(ex.Flatten().InnerException.Message);
            //            }
            //        }
            //    }
            //    catch (AggregateException ex)
            //    {
            //        System.Console.WriteLine(ex.Flatten().Message);
            //    }
            //}

            //System.Console.ReadLine();
        }
    }
}