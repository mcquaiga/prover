using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly ScreenManager _screenManager;

        public MainMenuViewModel(IEnumerable<IAppMainMenu> appMainMenus, ScreenManager screenManager,
            IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            _screenManager = screenManager;
            AppMainMenus = appMainMenus;
        }

        public IEnumerable<IAppMainMenu> AppMainMenus { get; }

        public async Task ActionCommand(Action startupAction)
        {
            await Task.Run(startupAction);
        }
    }
}