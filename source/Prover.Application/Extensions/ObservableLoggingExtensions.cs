using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace Prover.Application.ViewModels
{
    public static class ObservableLoggingExtensions
    {
        private static ILogger _logger = ProverLogging.CreateLogger("ObservableLogging");

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
            logger = logger ?? _logger;
            var dateTime = includeTime ? DateTime.Now.ToString() : string.Empty;
            return source.Do(x => logger.LogDebug($"{dateTime} {source.GetType()} - {message}"));
        }

        public static IObservable<T> LogDebug<T>(this IObservable<T> source, Func<T, string> message, ILogger logger = null, bool includeTime = false)
        {
            logger = logger ?? _logger;
            var dateTime = includeTime ? DateTime.Now.ToString() : string.Empty;
            return source.Do(x => logger.LogDebug($"{dateTime} {source.GetType()} - {message.Invoke(x)}"));
        }
    }
}