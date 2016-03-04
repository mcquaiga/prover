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

        private readonly IUnityContainer _container;
        
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private bool _userHasRequestedStop;
        private bool _isFirstVolumeTest = true;

        public InstrumentManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }
        public bool ShowButtons { get; }

        public VolumeViewModel(IUnityContainer container, bool showButtons = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowButtons = showButtons;
            _userHasRequestedStop = false;
        }

        public VolumeViewModel(IUnityContainer container, Instrument instrument, bool showButtons = true) : this(container, showButtons)
        {
            Instrument = instrument;
        }

        public Volume Volume
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Volume;
                }
                return Instrument?.Volume;
            }
        }

        public decimal AppliedInput
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Volume.AppliedInput;
                }
                else if (Instrument != null)
                {
                    return (decimal)Instrument?.Volume?.AppliedInput;
                }
                return 0.00m;
            }
            set
            {
                InstrumentManager.Instrument.Volume.AppliedInput = value;
                RaisePropertyChanges();
            }
        }

        public async void StartTestCommand()
        {
            try
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));

                if (!_isFirstVolumeTest) await InstrumentManager.DownloadVolumeItems();
                _isFirstVolumeTest = false;

                await InstrumentManager.StartVolumeTest();
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

        private void RaisePropertyChanges()
        {
            NotifyOfPropertyChange(() => AppliedInput);
            NotifyOfPropertyChange(() => Volume);
            NotifyOfPropertyChange(() => UnCorrectedPercentColour);
            NotifyOfPropertyChange(() => CorrectedPercentColour);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
            RaisePropertyChanges();
        }
    }
}
