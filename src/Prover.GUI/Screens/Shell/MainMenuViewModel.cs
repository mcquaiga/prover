using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.GUI.Common;
using Prover.GUI.Screens.Export;
using Prover.GUI.Screens.QAProver;
using Prover.GUI.Screens.RawItemAccess;

namespace Prover.GUI.Screens.Shell
{
    public class MainMenuViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;

        public MainMenuViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public async Task NewTestButton()
        {
            await ScreenManager.Change(_container, new StartTestViewModel(_container));
        }

        public async Task ExportRunButton()
        {
            await ScreenManager.Change(_container, new CreateCertificateViewModel(_container));
        }

        public async Task RawInstrumentAccessButton()
        {
            await ScreenManager.Change(_container, new InstrumentAccessViewModel(_container));
        }
    }
}