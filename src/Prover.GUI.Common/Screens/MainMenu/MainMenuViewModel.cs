using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly ScreenManager _screenManager;

        public MainMenuViewModel(IUnityContainer container, ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
            _screenManager = screenManager;
            AppMainMenus = container.ResolveAll<IAppMainMenu>().ToArray();
        }

        public IAppMainMenu[] AppMainMenus { get; }

        public async Task ActionCommand(Action startupAction)
        {
            await Task.Run(startupAction);
        }

        //public async Task NewTestButton()
        //{
        //    await _screenManager.ChangeScreen(IoC.Get<StartTestViewModel>());
        //}

        //public async Task ExportRunButton()
        //{
        //    await _screenManager.ChangeScreen(new CreateCertificateViewModel(_screenManager));
        //}

        //public async Task RawInstrumentAccessButton()
        //{
        //    await _screenManager.ChangeScreen(new InstrumentAccessViewModel(_screenManager));
        //}
    }
}