using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;

namespace Prover.Application.Extensions
{
    public static class ObservableLoggingExtensions
    {
        //private static ILogger _logger = ProverLogging.CreateLogger("ObservableLogging");
        private static readonly ILogger _logger = ProverLogging.CreateLogger("ObservableLogging");

        public static IObservable<Exception> LogErrors(this IObservable<Exception> source, string message = null, ILogger logger = null)
        {
            logger = logger ?? _logger;
            var time = DateTime.Now;

            if (logger != null)
                return source.Do(ex => logger.LogError(ex, $"{time} - {message}."));

            return source.Do(ex =>
            {
                logger.LogError(ex, $"{time} - {message}.");
            });
        }

        public static IObservable<T> LogErrors<T>(this IObservable<T> source, ILogger logger = null)
        {
            logger = logger ?? _logger;
            return source.Do(value => { }, ex => logger.LogError(ex, "An error occured in an observable."));

        }

        public static IObservable<T> LogDebug<T>(this IObservable<T> source, string message, ILogger logger = null, bool includeTime = false)
        {
            return source.LogDebug(x => message, logger, includeTime);
        }

        public static IObservable<T> LogDebug<T>(this IObservable<T> source, Func<T, string> message, ILogger logger, bool includeTime = false)
        {
            logger = logger ?? _logger;

            var dateTime = includeTime ? $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}: " : string.Empty;

            return source.Do(x => logger.LogDebug($"{dateTime}{source.GetType().Name} - {message.Invoke(x)}"));
        }

        public static IObservable<T> LogDebug<T>(this IObservable<T> source, Func<T, string> message)
        {

            var dateTime = $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}: ";

            return source.Do(x => Debug.WriteLine($"{dateTime}{source.GetType().Name} - {message.Invoke(x)}"));
        }
    }
}