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
using Prover.Core.VerificationTests;

namespace Prover.GUI.ViewModels
{
    public class VolumeViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;        
        private bool _isReportView;

        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }
        public bool ShowBeginTestButton { get; private set; } = true;
        public bool ShowStopTestButton { get; private set; } = false;

        public bool ShowAppliedInputTextBox => !_isReportView;
        public bool ShowAppliedInputDisplay => _isReportView;
        public bool ShowTestButtons => !_isReportView;

        public VolumeViewModel(IUnityContainer container, bool isReportView = false)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
           
            _isReportView = isReportView;
        }

        public VolumeViewModel(IUnityContainer container, Instrument instrument) : this(container, true)
        {
            Instrument = instrument;
        }

        public VolumeViewModel(IUnityContainer container, TestManager instrumentTestManager) : this(container, false)
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
            ToggleTestButtons();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));
            await InstrumentManager.StartVolumeTest();
            RaisePropertyChanges();
        }

        public void StopTestCommand()
        {
            ToggleTestButtons();
            InstrumentManager.StopVolumeTest();
            RaisePropertyChanges();  
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
