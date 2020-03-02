using System;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Screens
{
    public class ViewModelBase : ReactiveScreen, IDisposable
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly ScreenManager ScreenManager;
        protected Logger Log = LogManager.GetCurrentClassLogger();

        public ViewModelBase(ScreenManager screenManager, IEventAggregator eventAggregator)
        {
            ScreenManager = screenManager;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        protected ViewModelBase()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}