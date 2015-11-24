using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.GUI.Events;
using Prover.GUI.Views;

namespace Prover.GUI.ViewModels
{
    public class MainMenuViewModel : Conductor<object>.Collection.OneActive
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
    }
}
