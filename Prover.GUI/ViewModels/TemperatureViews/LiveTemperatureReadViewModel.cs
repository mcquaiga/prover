using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels.TemperatureViews
{
    public class LiveTemperatureReadViewModel : ReactiveScreen, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private InstrumentManager _instrumentManager;

        public LiveTemperatureReadViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public double LiveReadTemperature { get; set; }

        public async Task StartLiveReadCommand()
        {
            await _instrumentManager.StartLiveReadTemperature();
        }

        public async Task StopLiveReadCommand()
        {
            await _instrumentManager.StopLiveReadTemperature();
        }

        public void Handle(LiveReadEvent message)
        {
            LiveReadTemperature = message.LiveReadValue;
            NotifyOfPropertyChange(() => LiveReadTemperature);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }
    }
}
