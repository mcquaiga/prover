using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async void StartTestCommand()
        {
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting volume test..."));
            await InstrumentManager.StartVolumeTest();
            await Task.Run(() =>
            {
                do
                {
                    InstrumentManager.Instrument.Volume.PulseACount += InstrumentManager.AInputBoard.ReadInput();
                    NotifyOfPropertyChange(() => Volume.PulseACount);
                    InstrumentManager.Instrument.Volume.PulseBCount += InstrumentManager.AInputBoard.ReadInput();
                    NotifyOfPropertyChange(() => Volume.PulseBCount);
                } while (InstrumentManager.Instrument.Volume.UncPulseCount < InstrumentManager.Instrument.Volume.MaxUnCorrected());
            });
            
            await InstrumentManager.StopVolumeTest();
            NotifyOfPropertyChange(() => Volume);
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed volume test!"));
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
