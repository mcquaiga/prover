using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using System.Windows.Media;

namespace Prover.GUI.ViewModels
{
    public class VolumeViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;        
        private bool _userHasRequestedStop;

        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }
        public bool ShowBeginTestButton { get; private set; } = true;
        public bool ShowStopTestButton { get; private set; } = false;

        public VolumeViewModel(IUnityContainer container, bool showButtons = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            
            _userHasRequestedStop = false;

            if (!showButtons)
            {
                ShowBeginTestButton = false;
                ShowStopTestButton = false;
            }
        }

        public VolumeViewModel(IUnityContainer container, Instrument instrument) : this(container, false)
        {
            Instrument = instrument;
        }

        public VolumeViewModel(IUnityContainer container, TestManager instrumentTestManager) : this(container, true)
        {
            InstrumentManager = instrumentTestManager;
            Instrument = InstrumentManager.Instrument;
        }

        public Volume Volume
        {
            get
            {
                return Instrument?.Volume;
            }
        }
       
        public double AppliedInput
        {
            get
            {
                return Instrument?.Volume?.AppliedInput ?? 0.00;
            }
            set
            {
                Instrument.Volume.AppliedInput = value;
                RaisePropertyChanges();
            }
        }

        public async void StartTestCommand()
        {
            try
            {
                ToggleTestButtons();
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));

                await InstrumentManager.StartVolumeTest();

                //TODO: Move this into the above function .StartVolumeTest()
                await Task.Run(() =>
                {
                    do
                    {
                        InstrumentManager.Instrument.Volume.PulseACount += InstrumentManager.AInputBoard.ReadInput();
                        InstrumentManager.Instrument.Volume.PulseBCount += InstrumentManager.BInputBoard.ReadInput();
                        NotifyOfPropertyChange(() => Volume);
                    } while (InstrumentManager.Instrument.Volume.UncPulseCount < InstrumentManager.Instrument.Volume.MaxUnCorrected() || _userHasRequestedStop);
                });
                _userHasRequestedStop = false;
                await InstrumentManager.StopVolumeTest();

                RaisePropertyChanges();

                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed volume test!"));
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("An error occured: {0}", ex.Message), ex);
            }
        }

        public async void StopTestCommand()
        {
            ToggleTestButtons();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Finishing volume test..."));
            _userHasRequestedStop = true;
            if (InstrumentManager != null)
            {
                await InstrumentManager.StopVolumeTest();
                RaisePropertyChanges();
            }    
        }

        public Brush UnCorrectedPercentColour => Volume?.UnCorrectedHasPassed == true ? Brushes.Green : Brushes.Red;
        public Brush CorrectedPercentColour => Volume?.CorrectedHasPassed == true ? Brushes.Green : Brushes.Red;
        public Brush MeterDisplacementPercentColour => Volume?.MeterDisplacementHasPassed == true ? Brushes.Green : Brushes.Red;

        private void ToggleTestButtons()
        {
            ShowBeginTestButton = !ShowBeginTestButton;
            ShowStopTestButton = !ShowStopTestButton;
            RaisePropertyChanges();
        }

        private void RaisePropertyChanges()
        {
            NotifyOfPropertyChange(() => AppliedInput);
            NotifyOfPropertyChange(() => Volume);
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
            NotifyOfPropertyChange(() => ShowStopTestButton);
            NotifyOfPropertyChange(() => ShowBeginTestButton);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
            RaisePropertyChanges();
        }
    }
}
