﻿using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Action = System.Action;

namespace Prover.GUI.ViewModels
{
    class LiveReadViewModel : ReactiveScreen, IWindowSettings, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
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

        public ObservableCollection<LiveReadDisplay> LiveReadItems { get; set; } = new ObservableCollection<LiveReadDisplay>();

        private async Task DoLiveRead()
        {
            await _testManager.StartLiveRead();
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
            Application.Current.Dispatcher.Invoke(delegate
            {
                var item = LiveReadItems.FirstOrDefault(x => x.ItemNumber == message.ItemNumber);

                if (item == null)
                {
                    LiveReadItems.Add(new LiveReadDisplay { ItemNumber = message.ItemNumber, LiveReadValue = message.LiveReadValue });
                }
                else
                {
                    item.LiveReadValue = message.LiveReadValue;
                }

                NotifyOfPropertyChange(() => LiveReadItems);
            });

        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }

        public class LiveReadDisplay : ReactiveScreen
        {
            private decimal _readValue;

            public string Title
            {
                get
                {
                    return string.Format("Live Item #{0}", ItemNumber);
                }
            }

            public int ItemNumber { get; set; }

            public decimal LiveReadValue
            {
                get
                {
                    return _readValue;
                }
                set
                {
                    _readValue = value;
                    NotifyOfPropertyChange(() => LiveReadValue);
                }
            }
        }
    }
}
