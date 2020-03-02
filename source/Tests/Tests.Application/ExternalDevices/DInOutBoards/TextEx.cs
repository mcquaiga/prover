using System;
using Microsoft.Reactive.Testing;

namespace Application.ExternalDevices.DInOutBoards.Tests
{
    public static class TextEx
    {
        public static void AdvanceByMilliSeconds(this TestScheduler source, double seconds)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.AdvanceBy(TimeSpan.FromMilliseconds(seconds).Ticks);
        }

        public static void AdvanceBySeconds(this TestScheduler source, int seconds)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
        }
    }
}