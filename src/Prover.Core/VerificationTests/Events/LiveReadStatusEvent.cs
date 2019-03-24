using Prover.CommProtocol.Common.Items;
using System.Collections.Generic;
using System.Linq;

namespace Prover.Core.VerificationTests.Events
{
    public class LiveReadStatusEvent
    {
        public LiveReadStatusEvent(string headerMessage, Dictionary<ItemValue, AveragedReadingStabilizer> liveReadItems)
        {
            LiveReadItems = liveReadItems;
            HeaderMessage = headerMessage;

            PressureActual = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 8).Value.Latest();
            PressureTarget = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 8).Value.GaugeValue;

            TempActual = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 26).Value.Latest();
            TempTarget = LiveReadItems.FirstOrDefault(l => l.Key.Metadata.Number == 26).Value.GaugeValue;
        }

        public LiveReadStatusEvent(decimal pressureActual, decimal pressureTarget, decimal tempActual, decimal tempTarget)
        {
            PressureActual = pressureActual;
            PressureTarget = pressureTarget;
            TempActual = tempActual;
            TempTarget = tempTarget;
        }

        public decimal PressureActual { get; }
        public decimal PressureTarget { get; }
        public decimal TempActual { get; }
        public decimal TempTarget { get; }
        public Dictionary<ItemValue, AveragedReadingStabilizer> LiveReadItems { get; }
        public string HeaderMessage { get; }
    }
}
