using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.GUI.ViewModels.TestViews;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.Shell
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

        public async Task CreateCertificateButton()
        {
            await ScreenManager.Change(_container, new CreateCertificateViewModel(_container));
        }

        public async Task RawInstrumentAccessButton()
        {
            await ScreenManager.Change(_container, new InstrumentAccessViewModel(_container));
        }
    }
}
