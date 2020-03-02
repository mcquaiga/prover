using System;
using System.Reactive.Linq;

namespace Prover.Core.Shared.Extensions
{
    public static class RxExtensions
    {
        public static IObservable<T> StepInterval<T>(this IObservable<T> source, TimeSpan minDelay)
        {
            return source.Select(x =>
                Observable.Empty<T>()
                    .Delay(minDelay)
                    .StartWith(x)
            ).Concat();
        }
    }
}