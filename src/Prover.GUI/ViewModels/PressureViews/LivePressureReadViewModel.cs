using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prover.GUI.ViewModels.PressureViews
{
    public class LivePressureReadViewModel : ReactiveScreen, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private TestManager _instrumentManager;

        public LivePressureReadViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public decimal LiveReadPressure { get; set; }

        public async Task StartLiveReadCommand()
        {
            try
            {
                await _instrumentManager.StartLiveRead(8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured communicating with the instrument." + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButton.OK);
            }
        }

        public async Task StopLiveReadCommand()
        {
            await _instrumentManager.StopLiveRead();
        }

        public void Handle(LiveReadEvent message)
        {
            LiveReadPressure = message.LiveReadValue;
            NotifyOfPropertyChange(() => LiveReadPressure);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }
    }
}
