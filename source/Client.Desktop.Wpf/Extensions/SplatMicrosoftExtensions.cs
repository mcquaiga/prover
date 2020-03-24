using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Splat;
using Squirrel;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using SplatILogger = Splat.ILogger;
using SplatLogLevel = Splat.LogLevel;

namespace Client.Desktop.Wpf.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="MicrosoftDependencyResolver" />.
    /// </summary>
    [SuppressMessage("Reliability", "CA2000", Justification = "Intentional")]
    public static class SplatMicrosoftExtensions
    {
        public static void AddSplatLogging(this IServiceCollection services)
        {
            services.AddSingleton<SplatILogger>(c =>
            {
                var logger = c.GetService<ILoggerFactory>().CreateLogger("ReactiveUI");
                return new RxLogging(logger);
            });
        }

        /// <summary>
        ///     Initializes an instance of <see cref="MicrosoftDependencyResolver" /> that overrides the default
        ///     <see cref="Splat.Locator" />.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" />.</param>
        public static void UseMicrosoftDependencyResolver(this IServiceCollection serviceCollection) =>
            Locator.SetLocator(new MicrosoftDependencyResolver(serviceCollection));

        /// <summary>
        ///     Initializes an instance of <see cref="MicrosoftDependencyResolver" /> that overrides the default
        ///     <see cref="Locator" />
        ///     with a built <see cref="IServiceProvider" />.
        /// </summary>
        /// <remarks>
        ///     If there is already a <see cref="MicrosoftDependencyResolver" /> serving as the
        ///     <see cref="Locator.Current" />, it'll instead update it to use the specified
        ///     <paramref name="serviceProvider" />.
        /// </remarks>
        /// <param name="serviceProvider">The <see cref="IServiceProvider" />.</param>
        public static void UseMicrosoftDependencyResolver(this IServiceProvider serviceProvider)
        {
            if (Locator.Current is MicrosoftDependencyResolver resolver)
                resolver.UpdateContainer(serviceProvider);
            else
                Locator.SetLocator(new MicrosoftDependencyResolver(serviceProvider));
        }
    }

    public class RxLogging : SplatILogger
    {
        private static readonly Dictionary<SplatLogLevel, LogLevel> _levels = new Dictionary<SplatLogLevel, LogLevel>
        {
            {SplatLogLevel.Debug, LogLevel.Debug},
            {SplatLogLevel.Error, LogLevel.Error},
            {SplatLogLevel.Fatal, LogLevel.Critical},
            {SplatLogLevel.Info, LogLevel.Information},
            {SplatLogLevel.Warn, LogLevel.Warning}
        };


        private readonly ILogger _dotnetLogger;

        public RxLogging(ILogger dotnetLogger) => _dotnetLogger = dotnetLogger;

        public SplatLogLevel Level => SplatLogLevel.Debug;

        public void Write([Localizable(false)] string message, SplatLogLevel logLevel)
        {
            _dotnetLogger.Log(_levels[logLevel], message);
        }

        public void Write(Exception exception, [Localizable(false)] string message, SplatLogLevel logLevel)
        {
            _dotnetLogger.Log(_levels[logLevel], exception, message);
        }

        public void Write([Localizable(false)] string message, [Localizable(false)] Type type, SplatLogLevel logLevel)
        {
            _dotnetLogger.Log(_levels[logLevel], message, type);
        }

        public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type,
            SplatLogLevel logLevel)
        {
            _dotnetLogger.Log(_levels[logLevel], exception, message, type);
        }
    }

    public class ProverLogManager : ILogManager
    {
        private readonly ILoggerFactory _loggerFactory;
        private WrappingFullLogger _updateManagerLog;

        public ProverLogManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            var logger = _loggerFactory.CreateLogger(typeof(IUpdateManager));
            var dotNetToSplatLogger = new RxLogging(logger);
            _updateManagerLog = new WrappingFullLogger(dotNetToSplatLogger);
        }

        public IFullLogger GetLogger(Type type)
        {
            if (type is IUpdateManager)
                return _updateManagerLog;

            var logger = _loggerFactory.CreateLogger(type);
            var dotNetToSplatLogger = new RxLogging(logger);
            return new WrappingFullLogger(dotNetToSplatLogger);
        }
    }
}