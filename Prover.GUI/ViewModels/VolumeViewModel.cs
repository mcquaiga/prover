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
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels
{
    public class VolumeViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {

        private readonly IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }

        public VolumeViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public Volume Volume
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Volume;
                }
                return null;
            }
        }

        public double AppliedInput
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Volume.AppliedInput;
                }
                return 0.00;
            }
            set
            {
                InstrumentManager.Instrument.Volume.AppliedInput = value;
                NotifyOfPropertyChange(() => Volume);
            }
        }

        public async void StartTestCommand()
        {
            try
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));
                await InstrumentManager.StartVolumeTest();
                await Task.Run(() =>
                {
                    do
                    {
                        InstrumentManager.Instrument.Volume.PulseACount += InstrumentManager.AInputBoard.ReadInput();
                        InstrumentManager.Instrument.Volume.PulseBCount += InstrumentManager.BInputBoard.ReadInput();
                        NotifyOfPropertyChange(() => Volume);
                    } while (InstrumentManager.Instrument.Volume.UncPulseCount < InstrumentManager.Instrument.Volume.MaxUnCorrected());
                });

                await InstrumentManager.StopVolumeTest();
                NotifyOfPropertyChange(() => AppliedInput);
                NotifyOfPropertyChange(() => Volume);
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed volume test!"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void StopTestCommand()
        {
            if (InstrumentManager != null)
                await InstrumentManager.StopVolumeTest();
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
            NotifyOfPropertyChange(() => Volume);
        }
    }
}
