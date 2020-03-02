using System.Collections.Generic;
using System.Linq;

namespace Devices.Communications.IO
{
    public static class CommPorts
    {
        public static string IrDAName => "IrDA";

        public static IEnumerable<string> GetPorts()
        {
            var ports = SerialPort.GetPortNames().ToList();
            ports.Add(IrDAName);
            return ports;
        }
    }
}