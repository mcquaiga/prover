using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.SerialProtocol;

namespace Prover.Core.Communication
{
    public class Communications
    {
        public static ICommPort CreateCommPortObject(string commName, BaudRateEnum baudRate)
        {
            if (!GetCommPortList().Contains(commName)) return null;

            ICommPort commPort;
            if (commName == "IrDA")
            {
                commPort = new IrDAPort();
            }
            else
            {
                commPort = new SerialPort(commName, baudRate);
            }
            return commPort;
        }

        public static List<string> GetCommPortList()
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
            ports.Add("IrDA");
            return ports;
        }

    }
}
