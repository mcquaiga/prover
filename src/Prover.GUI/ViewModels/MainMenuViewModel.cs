using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Prover.GUI.Events;
using Prover.GUI.Views;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
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
            await ScreenManager.Change(_container, new InstrumentConnectViewModel(_container));
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
