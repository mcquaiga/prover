namespace Prover.Client.WPF.Screens.QAProver
{
    internal class LiveReadViewModel : ReactiveScreen, IWindowSettings, IHandle<LiveReadEvent>,
        IHandle<InstrumentUpdateEvent>
    {
        private readonly IContainer _container;
        private QaRunTestManager _qaRunTestManager;

        public LiveReadViewModel(IContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            _qaRunTestManager = _container.Resolve<QaRunTestManager>();
        }

        public ObservableCollection<LiveReadDisplay> LiveReadItems { get; set; } =
            new ObservableCollection<LiveReadDisplay>();

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

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        public async Task CloseCommand()
        {
            TryClose();
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            //_instrumentManager = message.InstrumentManager;
        }

        public void Handle(LiveReadEvent message)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var item = LiveReadItems.FirstOrDefault(x => x.ItemNumber == message.ItemNumber);

                if (item == null)
                    LiveReadItems.Add(new LiveReadDisplay
                    {
                        ItemNumber = message.ItemNumber,
                        LiveReadValue = message.LiveReadValue
                    });
                else
                    item.LiveReadValue = message.LiveReadValue;

                NotifyOfPropertyChange(() => LiveReadItems);
            });
        }

        private async Task DoLiveRead()
        {
        }

        public class LiveReadDisplay : ReactiveScreen
        {
            private decimal _readValue;

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

            public string Title
            {
                get { return string.Format("Live Item #{0}", ItemNumber); }
            }
        }
    }
}