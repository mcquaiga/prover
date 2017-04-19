using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using Splat;
using ILogger = NLog.ILogger;
using IScreen = ReactiveUI.IScreen;
using LogManager = NLog.LogManager;

namespace Prover.Client.Framework.Screens
{
    public class ViewModelBase : ReactiveScreen
    {
        public IScreenManager ScreenManager { get; }
        protected ILogger Logger;

        public ViewModelBase(IScreenManager screenManager) : this(null, screenManager) { }

        public ViewModelBase(ILogger logger, IScreenManager screenManager)
        {
            ScreenManager = screenManager;
            Logger = logger ?? Locator.Current.GetService<ILogger>();
        }

        protected ViewModelBase()
        {
            ScreenManager = Locator.Current.GetService<IScreenManager>();
            Logger = Locator.Current.GetService<ILogger>();
        }
    }
}