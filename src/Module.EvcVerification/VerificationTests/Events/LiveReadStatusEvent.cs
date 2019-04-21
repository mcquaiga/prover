using System.Collections.Generic;
using System.Linq;

namespace Module.EvcVerification.VerificationTests.Events
{
    public class LiveReadStatusEvent
    {
        public LiveReadStatusEvent(string headerMessage, Dictionary<ItemValue, AveragedReadingStabilizer> liveReadItems)
        {
            LiveReadItems = liveReadItems;
            HeaderMessage = headerMessage;
           
            if (LiveReadItems.Any(l => l.Key.Metadata.Number == 8))
            {
                PressureActual = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 8).Value.Latest();
                PressureTarget = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 8).Value.GaugeValue;
            }
            
            if (LiveReadItems.Any(l => l.Key.Metadata.Number == 26))
            {
                TempActual = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 26).Value.Latest();
                TempTarget = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 26).Value.GaugeValue;
            }
           
        }

        public LiveReadStatusEvent(double pressureActual, double pressureTarget, double tempActual, double tempTarget)
        {
            PressureActual = pressureActual;
            PressureTarget = pressureTarget;
            TempActual = tempActual;
            TempTarget = tempTarget;
        }

        public double PressureActual { get; }
        public double PressureTarget { get; }
        public double TempActual { get; }
        public double TempTarget { get; }
        public Dictionary<ItemValue, AveragedReadingStabilizer> LiveReadItems { get; }
        public string HeaderMessage { get; }
    }
}
