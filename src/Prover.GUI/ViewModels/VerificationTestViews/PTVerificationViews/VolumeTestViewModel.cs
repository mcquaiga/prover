using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.EVCTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
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

        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument => Volume.Instrument;

        public VolumeTestViewModel(IUnityContainer container, Core.Models.Instruments.VolumeTest volumeTest)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            Volume = volumeTest;
        }

        public Prover.Core.Models.Instruments.VolumeTest Volume { get; private set; }
       
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
        public decimal? TrueUncorrected => decimal.Round(Volume.TrueUncorrected.Value, 4);
        public decimal? StartUncorrected => Volume.ItemValues.Uncorrected();
        public decimal? EndUncorrected => Volume.AfterTestItemValues.Uncorrected();
        public decimal? StartCorrected => Volume.ItemValues.Corrected();
        public decimal? EndCorrected => Volume.AfterTestItemValues.Corrected();
        public decimal? EvcUncorrected => Instrument.EvcUncorrected(Volume.ItemValues, Volume.AfterTestItemValues);
        public decimal? EvcCorrected => Instrument.EvcCorrected(Volume.ItemValues, Volume.AfterTestItemValues);

        public int UncorrectedPulseCount => Volume.UncPulseCount;
        public int CorrectedPulseCount => Volume.CorPulseCount;

        public Brush UnCorrectedPercentColour => Volume?.UnCorrectedHasPassed == true ? Brushes.White : (SolidColorBrush)(new BrushConverter().ConvertFrom("#DC6156"));
        public Brush CorrectedPercentColour => Volume?.CorrectedHasPassed == true ? Brushes.White : (SolidColorBrush)(new BrushConverter().ConvertFrom("#DC6156"));
        public Brush MeterDisplacementPercentColour => Brushes.Green; // Volume.DriveType.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;

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
            NotifyOfPropertyChange(() => UncorrectedPulseCount);
            NotifyOfPropertyChange(() => CorrectedPulseCount);
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
        }

        public void Handle(VerificationTestEvent message)
        {
            RaisePropertyChanges();
        }
    }
}