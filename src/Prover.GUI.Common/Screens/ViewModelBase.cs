using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Common.Screens
{
    public class ViewModelBase : ReactiveScreen
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
    }
}