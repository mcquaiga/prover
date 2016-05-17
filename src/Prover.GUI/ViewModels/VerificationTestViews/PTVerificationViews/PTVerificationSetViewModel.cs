using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen
    {
        private IUnityContainer _container;
        private TestManager _testManager;

        public PTVerificationSetViewModel(IUnityContainer container, VerificationTest verificationTest)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (_container.IsRegistered<TestManager>())
                _testManager = _container.Resolve<TestManager>();
             
            VerificationTest = verificationTest;
            CreateViews();
        }

        private void CreateViews()
        {
            if (VerificationTest.Instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
                SuperFactorTestViewModel = new SuperTestViewModel(_container, VerificationTest.SuperFactorTest);
            }                

            if (VerificationTest.Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
            }

            if (VerificationTest.Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
            }

            if (VerificationTest.VolumeTest != null)
            {
                VolumeTestViewModel = new VolumeTestViewModel(_container, _testManager);
            }
        }

        public string Level => $"Level {VerificationTest.TestNumber + 1}";
        public bool ShowVolumeTestViewModel => VolumeTestViewModel != null;
        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestViewModel { get; private set; }
        public SuperTestViewModel SuperFactorTestViewModel { get; private set; }
        public VolumeTestViewModel VolumeTestViewModel { get; private set; }

        public VerificationTest VerificationTest { get; private set; }

        public async Task DownloadItems()
        {
            await _testManager.DownloadVerificationTestItems(VerificationTest.TestNumber);
            _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
        }
        
        public void LiveReadItemsCommand()
        {
            var viewmodel = new LiveReadViewModel(_container);
            ScreenManager.ShowDialog(_container, viewmodel);
        }
    }
}
