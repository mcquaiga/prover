using System;
using System.Collections.Generic;
using System.Linq;
using RJCP.IO.Ports;

namespace Prover.ProtocolMonitor.Model
{
    [Serializable]
    public class SerialPortSettingsModel : SingletonBase<SerialPortSettingsModel>
    {
        public int[] GetDataBits = {5, 6, 7, 8};

        public string BaudRateName { get; set; }
        public int BaudRateValue { get; set; }
        public string Description { get; set; }
        public string DeviceId { get; set; }
        public string DeviceInfo { get; set; }
        public string LineEndingChars { get; set; }
        public string LineEndingName { get; set; }
        public string ParityName { get; set; }
        public Parity ParityValue { get; set; }
        public string StopBitsName { get; set; }
        public StopBits StopBitsValue { get; set; }

        public List<SerialPortSettingsModel> GetBaudRates()
        {
            var returnBaudRates = new List<SerialPortSettingsModel>
            {
                new SerialPortSettingsModel {BaudRateName = "4800 baud", BaudRateValue = 4800},
                new SerialPortSettingsModel {BaudRateName = "9600 baud", BaudRateValue = 9600},
                new SerialPortSettingsModel {BaudRateName = "19200 baud", BaudRateValue = 19200},
                new SerialPortSettingsModel {BaudRateName = "38400 baud", BaudRateValue = 38400},
                new SerialPortSettingsModel {BaudRateName = "57600 baud", BaudRateValue = 57600},
                new SerialPortSettingsModel {BaudRateName = "115200 baud", BaudRateValue = 115200},
                new SerialPortSettingsModel {BaudRateName = "230400 baud", BaudRateValue = 230400}
            };
            return returnBaudRates;
        }

        public List<SerialPortSettingsModel> GetCommPorts()
        {
            var ports = SerialPortStream.GetPortDescriptions();

            return Enumerable.ToList<SerialPortSettingsModel>(ports.Select(device => new SerialPortSettingsModel
            {
                DeviceId = device.Port,
                Description = device.Description,
                DeviceInfo = $"{device.Port} - {device.Description}"
            }));
        }

        public List<SerialPortSettingsModel> GetLineEndings()
        {
            var returnLineEndings = new List<SerialPortSettingsModel>
            {
                new SerialPortSettingsModel {LineEndingName = "No line ending", LineEndingChars = ""},
                new SerialPortSettingsModel {LineEndingName = "Newline", LineEndingChars = "\n"},
                new SerialPortSettingsModel
                {
                    LineEndingName = "Carriage return",
                    LineEndingChars = "\r"
                },
                new SerialPortSettingsModel
                {
                    LineEndingName = "Both NL & CR",
                    LineEndingChars = "\r\n"
                }
            };
            return returnLineEndings;
        }

        public List<SerialPortSettingsModel> GetParities()
        {
            return new List<SerialPortSettingsModel>
            {
                new SerialPortSettingsModel {ParityName = "Even", ParityValue = Parity.Even},
                new SerialPortSettingsModel {ParityName = "Mark", ParityValue = Parity.Mark},
                new SerialPortSettingsModel {ParityName = "None", ParityValue = Parity.None},
                new SerialPortSettingsModel {ParityName = "Odd", ParityValue = Parity.Odd},
                new SerialPortSettingsModel {ParityName = "Space", ParityValue = Parity.Space}
            };
            
        }

        public List<SerialPortSettingsModel> GetStopBits()
        {
            return new List<SerialPortSettingsModel>
            {
                new SerialPortSettingsModel {StopBitsName = "One", StopBitsValue = StopBits.One},
                new SerialPortSettingsModel {StopBitsName = "OnePointFive", StopBitsValue = StopBits.One5},
                new SerialPortSettingsModel {StopBitsName = "Two", StopBitsValue = StopBits.Two}
            };
        }
    }
}