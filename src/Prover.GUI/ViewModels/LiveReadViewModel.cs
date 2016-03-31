using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prover.GUI.ViewModels
{
    class LiveReadViewModel : ReactiveScreen, IWindowSettings, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private TestManager _instrumentManager;
        private TestManager _testManager;

        public LiveReadViewModel(IUnityContainer container, int itemNumber)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            _testManager = _container.Resolve<TestManager>();
            ItemNumber = itemNumber;

            Task.Run(() => DoLiveRead());
        }

        private async Task DoLiveRead()
        {
            await _testManager.StartLiveRead(ItemNumber);
        }

        public string Title
        {
            get
            {
                return string.Format("Live Item #{0}", ItemNumber);
            }
        }
        public int ItemNumber { get; }
        public decimal LiveReadValue { get; set; }

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.MinWidth = 450;
                settings.Title = "Live Read";
                return settings;
            }
        }

        public async Task CloseCommand()
        {
            await _testManager.StopLiveRead();
            this.TryClose();
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        public void Handle(LiveReadEvent message)
        {
            LiveReadValue = message.LiveReadValue;
            NotifyOfPropertyChange(() => LiveReadValue);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }
    }
}
