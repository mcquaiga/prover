using Microsoft.Extensions.Logging;
using System;

namespace Prover.Application
{
    public class ProverLogging
    {
        private static ILoggerFactory _Factory = null;

        public static ILoggerFactory LogFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = CreateDefaultFactory();

                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        private static ILoggerFactory CreateDefaultFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.AddConsole();
            });
            //LoggerFactory.Create(builder => { 
            //builder
            //    .AddFilter("Microsoft", LogLevel.Warning)
            //    .AddFilter("System", LogLevel.Warning)
            //    .AddFilter("Prover.", LogLevel.Debug)
            //    .AddFilter("Devices", LogLevel.Information)
            //    .AddFilter("Client.", LogLevel.Trace)
            //    .AddDebug()
            //    .AddConsole(); });
        }

        public static ILogger CreateLogger(string categoryName) => LogFactory.CreateLogger(categoryName);
        public static ILogger CreateLogger(Type type) => LogFactory.CreateLogger(type);
        public static ILogger<T> CreateLogger<T>() => LogFactory.CreateLogger<T>();
    }
}