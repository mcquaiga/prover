using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Prover.CommProtocol.Common.IO;

namespace Prover.Core.Communication
{
    //public class Communications
    //{
    //    public static CommPort CreateCommPortObject(string commName, BaudRateEnum baudRate)
    //    {
    //        CommPort commPort;

    //        if (commName == "IrDA")
    //        {
    //            commPort = new IrDAPort();
    //        }
    //        else
    //        {
    //            if (!GetCommPortList().Contains(commName)) throw new PortNotFoundException(commName);

    //            commPort = new SerialPort(commName, baudRate);
    //        }

    //        return commPort;
    //    }

    //    public static List<string> GetCommPortList()
    //    {
    //        var ports = SerialPort.GetPortNames().ToList();
    //        return ports;
    //    }
    //}

    //public class PortNotFoundException : Exception
    //{
    //    public PortNotFoundException(string commName)
    //        : base(string.Format("A port with the name {0} couldn't be found.", commName))
    //    {
    //    }
    //}
}