﻿using Prover.SerialProtocol;

namespace Prover.Core.Settings
{
    public interface ISettings
    {
        string InstrumentCommPort { get; set;}
        int InstrumentBaudRate { get; set; }
        string TachCommPort { get; set; }
    }

    public class Settings
    {
        public string InstrumentCommPort { get; set; }
        public BaudRateEnum InstrumentBaudRate { get; set; }
        public string TachCommPort { get; set; }
    }
}