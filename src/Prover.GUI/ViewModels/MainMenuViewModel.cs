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

        public void NewTestButton()
        {
            _container.Resolve<IEventAggregator>().PublishOnUIThread(new ScreenChangeEvent(new NewTestViewModel(_container)));
        }

        public void CreateCertificateButton()
        {
            _container.Resolve<IEventAggregator>().PublishOnUIThread(new ScreenChangeEvent(new CreateCertificateViewModel(_container)));
        }

        public void RawInstrumentAccessButton()
        {
            _container.Resolve<IEventAggregator>().PublishOnUIThread(new ScreenChangeEvent(new InstrumentAccessViewModel(_container)));
        }
    }
}
