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
            ICommPort commPort;

            if (commName == "IrDA")
            {
                commPort = new IrDAPort();
            }
            else
            {
                if (!GetCommPortList().Contains(commName)) throw new PortNotFoundException(commName);

                commPort = new SerialPort(commName, baudRate);
            }

            return commPort;
        }

        public static List<string> GetCommPortList()
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
            return ports;
        }
    }

    public class PortNotFoundException : Exception
    {
        public PortNotFoundException(string commName) : base(string.Format("A port with the name {0} couldn't be found.", commName))
        {

        }
    }

}
