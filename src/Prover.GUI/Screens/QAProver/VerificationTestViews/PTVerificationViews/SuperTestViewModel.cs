using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common.Events;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class SuperTestViewModel : BaseTestViewModel
    {
        private readonly IUnityContainer _container;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public SuperTestViewModel(IUnityContainer container, SuperFactorTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public decimal? EVCUnsqrFactor => (Test as SuperFactorTest).EvcUnsqrFactor;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => EVCUnsqrFactor);
        }
    }
}