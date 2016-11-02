using System.Windows.Media;
using Caliburn.Micro;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public interface IAppMainMenu
    {
        ImageSource IconSource { get; }
        string AppTitle { get; }
        Action ClickAction { get; }
    }

    public abstract class AppMainMenuBase : IAppMainMenu
    {
        protected IEventAggregator EventAggregator;
        protected ScreenManager ScreenManager;

        protected AppMainMenuBase(ScreenManager screenManager, IEventAggregator eventAggregator)
        {
            ScreenManager = screenManager;
            EventAggregator = eventAggregator;
        }

        public abstract ImageSource IconSource { get; }
        public abstract string AppTitle { get; }
        public abstract Action ClickAction { get; }
    }
}