using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.EVCTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class VolumeTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;        
        private bool _isReportView;

        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }

        public bool ShowBeginTestButton { get; private set; } = true;
        public bool ShowStopTestButton { get; private set; } = false;

        public bool ShowTestButtons => !_isReportView;

        public VolumeTestViewModel(IUnityContainer container, bool isReportView = false)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
           
            _isReportView = isReportView;
        }

        public VolumeTestViewModel(IUnityContainer container, Instrument instrument) : this(container, true)
        {
            Instrument = instrument;
        }

        public VolumeTestViewModel(IUnityContainer container, TestManager instrumentTestManager) : this(container, false)
        {
            InstrumentManager = instrumentTestManager;
            Instrument = InstrumentManager.Instrument;
        }

        public Prover.Core.Models.Instruments.VolumeTest Volume
        {
            get
            {
                return Instrument.VerificationTests.FirstOrDefault(x => x.VolumeTest != null).VolumeTest;
            }
        }
       
        public decimal AppliedInput
        {
            get
            {
                return Volume.AppliedInput;
            }
            set
            {
                Volume.AppliedInput = value;
                RaisePropertyChanges();
            }
        }

        public string DriveRateDescription => Instrument.DriveRateDescription();
        public string UnCorrectedMultiplierDescription => Instrument.UnCorrectedMultiplierDescription();
        public string CorrectedMultiplierDescription => Instrument.CorrectedMultiplierDescription();
        public decimal? TrueCorrected => decimal.Round(Volume.TrueCorrected.Value, 4);
        public decimal? StartUncorrected => Volume.ItemValues.Uncorrected();
        public decimal? EndUncorrected => Volume.AfterTestItemValues.Uncorrected();
        public decimal? StartCorrected => Volume.ItemValues.Corrected();
        public decimal? EndCorrected => Volume.AfterTestItemValues.Corrected();
        public decimal? EvcUncorrected => Instrument.EvcUncorrected(Volume.ItemValues, Volume.AfterTestItemValues);
        public decimal? EvcCorrected => Instrument.EvcCorrected(Volume.ItemValues, Volume.AfterTestItemValues);

        public int UncorrectedPulseCount => Volume.UncPulseCount;
        public int CorrectedPulseCount => Volume.CorPulseCount;


        public async Task StartTestCommand()
        {
            ToggleTestButtons();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));
            await InstrumentManager.VolumeTestManager.StartVolumeTest();
            RaisePropertyChanges();
        }

        public void StopTestCommand()
        {
            ToggleTestButtons();
            InstrumentManager.VolumeTestManager.StopVolumeTest();
            RaisePropertyChanges();  
        }

        public Brush UnCorrectedPercentColour => Volume?.UnCorrectedHasPassed == true ? Brushes.White : (SolidColorBrush)(new BrushConverter().ConvertFrom("#DC6156"));
        public Brush CorrectedPercentColour => Volume?.CorrectedHasPassed == true ? Brushes.White : (SolidColorBrush)(new BrushConverter().ConvertFrom("#DC6156"));
        public Brush MeterDisplacementPercentColour => Brushes.Green; // Volume.DriveType.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;

        private void ToggleTestButtons()
        {
            ShowBeginTestButton = !ShowBeginTestButton;
            ShowStopTestButton = !ShowStopTestButton;
            RaisePropertyChanges();
        }

        private void RaisePropertyChanges()
        {
            NotifyOfPropertyChange(() => AppliedInput);
            NotifyOfPropertyChange(() => TrueCorrected);
            NotifyOfPropertyChange(() => Volume);
            NotifyOfPropertyChange(() => StartUncorrected);
            NotifyOfPropertyChange(() => EndUncorrected);
            NotifyOfPropertyChange(() => StartCorrected);
            NotifyOfPropertyChange(() => EndCorrected);
            NotifyOfPropertyChange(() => EvcUncorrected);
            NotifyOfPropertyChange(() => EvcCorrected);
            NotifyOfPropertyChange(() => StartCorrected);            
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
            NotifyOfPropertyChange(() => ShowStopTestButton);
            NotifyOfPropertyChange(() => ShowBeginTestButton);
        }

        public void Handle(VerificationTestEvent message)
        {
            RaisePropertyChanges();
        }
    }
}