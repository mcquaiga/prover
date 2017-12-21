namespace Prover.Core.Settings
{
    public class LocalSettings
    {
        public string TachCommPort { get; set; }
        public bool TachIsNotUsed { get; set; }

        public string LastClientSelected { get; set; }

        public string LastInstrumentTypeUsed { get; set; }
        public string InstrumentCommPort { get; set; }
        public int InstrumentBaudRate { get; set; }
        public bool InstrumentUseIrDaPort { get; set; }
        public double WindowHeight { get; set; } = 800;
        public double WindowWidth { get; set; } = 800;
        public string WindowState { get; set; } = "Normal";
    }
}