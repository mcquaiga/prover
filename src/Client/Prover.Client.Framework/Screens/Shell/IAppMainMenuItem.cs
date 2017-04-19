using System.Windows.Media;
using Caliburn.Micro;
using Action = System.Action;

namespace Prover.Client.Framework.Screens.Shell
{
    public interface IAppMainMenuItem
    {
        string AppTitle { get; }
        Action ClickAction { get; }
        ImageSource IconSource { get; }
    }

    public abstract class AppMainMenuItemBase : IAppMainMenuItem
    {
        protected IEventAggregator EventAggregator;
        protected IScreenManager ScreenManager;

        protected AppMainMenuItemBase(IScreenManager screenManager, IEventAggregator eventAggregator)
        {
            ScreenManager = screenManager;
            EventAggregator = eventAggregator;
        }

        public abstract string AppTitle { get; }
        public abstract Action ClickAction { get; }

        public abstract ImageSource IconSource { get; }
    }
}