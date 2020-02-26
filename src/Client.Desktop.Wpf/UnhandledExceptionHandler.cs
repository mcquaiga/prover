using System;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace Client.Desktop.Wpf
{
    public class UnhandledExceptionHandler
    {
        private readonly ILogger<UnhandledExceptionHandler> _logger;

        public UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger)
        {
            _logger = logger;

            System.Windows.Application.Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            _logger.LogError(ex, ex.Message);
        }

        private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            _logger.LogError(ex, ex.Message);
            e.Handled = true;
        }

    }
}