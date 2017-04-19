using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Client.Framework;
using Prover.Client.Framework.Screens;
using Prover.Client.Framework.Screens.Shell;
using Action = System.Action;

namespace Prover.Client.WPF.Screens.Shell
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IScreenManager _screenManager;

        public MainMenuViewModel(IScreenManager screenManager, IEnumerable<IAppMainMenuItem> appMainMenus) : base(screenManager)
        {
            _screenManager = screenManager;           
            AppMainMenus = appMainMenus;
        }

        public IEnumerable<IAppMainMenuItem> AppMainMenus { get; }

        public async Task ActionCommand(Action startupAction)
        {
            await Task.Run(startupAction);
        }
    }
}