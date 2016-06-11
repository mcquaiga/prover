using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.Rotary;
using Prover.GUI.Events;

namespace Prover.GUI.Screens.QAProver
{
    internal class LiveReadViewModel : ReactiveScreen, IWindowSettings, IHandle<LiveReadEvent>,
        IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private RotaryTestManager _instrumentManager;
        private TestManager _testManager;

        public LiveReadViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            _testManager = _container.Resolve<TestManager>();
        }

        public ObservableCollection<LiveReadDisplay> LiveReadItems { get; set; } =
            new ObservableCollection<LiveReadDisplay>();

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }

        public void Handle(LiveReadEvent message)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var item = LiveReadItems.FirstOrDefault(x => x.ItemNumber == message.ItemNumber);

                if (item == null)
                {
                    LiveReadItems.Add(new LiveReadDisplay
                    {
                        ItemNumber = message.ItemNumber,
                        LiveReadValue = message.LiveReadValue
                    });
                }
                else
                {
                    item.LiveReadValue = message.LiveReadValue;
                }

                NotifyOfPropertyChange(() => LiveReadItems);
            });
        }

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

        private async Task DoLiveRead()
        {
        }

        public async Task CloseCommand()
        {
            TryClose();
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        public class LiveReadDisplay : ReactiveScreen
        {
            private decimal _readValue;

            public string Title
            {
                get { return string.Format("Live Item #{0}", ItemNumber); }
            }

            public int ItemNumber { get; set; }

            public decimal LiveReadValue
            {
                get { return _readValue; }
                set
                {
                    _readValue = value;
                    NotifyOfPropertyChange(() => LiveReadValue);
                }
            }
        }
    }
}