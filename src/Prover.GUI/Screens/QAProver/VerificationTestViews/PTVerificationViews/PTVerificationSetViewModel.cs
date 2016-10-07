using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;
        private readonly QaRunTestManager _qaRunTestManager;

        public PTVerificationSetViewModel(IUnityContainer container, VerificationTest verificationTest)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (_container.IsRegistered<QaRunTestManager>())
                _qaRunTestManager = _container.Resolve<QaRunTestManager>();

            VerificationTest = verificationTest;
            CreateViews();
        }

        public string Level => $"Level {VerificationTest.TestNumber + 1}";
        public bool ShowVolumeTestViewModel => VolumeTestViewModel != null;
        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestViewModel { get; private set; }
        public SuperTestViewModel SuperFactorTestViewModel { get; private set; }
        public VolumeTestViewModel VolumeTestViewModel { get; private set; }

        public VerificationTest VerificationTest { get; }

        private void CreateViews()
        {
            if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
                SuperFactorTestViewModel = new SuperTestViewModel(_container, VerificationTest.SuperFactorTest);
            }

            if (VerificationTest.Instrument.CompositionType == CorrectorType.T)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
            }

            if (VerificationTest.Instrument.CompositionType == CorrectorType.P)
            {
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
            }

            if (VerificationTest.VolumeTest != null)
            {
                VolumeTestViewModel = new VolumeTestViewModel(_container, VerificationTest.VolumeTest);
            }
        }

        public async Task RunTest()
        {
            await _qaRunTestManager.RunTest(VerificationTest.TestNumber);
            _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
        }
    }
}