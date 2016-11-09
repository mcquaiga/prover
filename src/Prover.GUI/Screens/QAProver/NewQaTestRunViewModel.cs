using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Akka.Util.Internal;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver
{
    public class NewQaTestRunViewModel : ViewModelBase, IHandle<SettingsChangeEvent>
    {
        public NewQaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            /*
             Setup Instruments list
             */
            Instruments.GetAll().ForEach(
                x => InstrumentTypes.Add(new SelectableInstrumentType { Instrument = x, IsSelected = false}));

            SetLastSelectedInstrumentType();
        }

        private void SetLastSelectedInstrumentType()
        {
            var lastSelected = InstrumentTypes.FirstOrDefault(
                i => i.Instrument.Name == SettingsManager.SettingsInstance.LastInstrumentTypeUsed);
            if (lastSelected != null) lastSelected.IsSelected = true;
        }

        private ReactiveList<SelectableInstrumentType> _instrumentTypes = new ReactiveList<SelectableInstrumentType>();
        public ReactiveList<SelectableInstrumentType> InstrumentTypes
        {
            get { return _instrumentTypes; }
            set { this.RaiseAndSetIfChanged(ref _instrumentTypes, value); }
        }

        public InstrumentType SelectedInstrument => InstrumentTypes.FirstOrDefault(i => i.IsSelected)?.Instrument;

        public int BaudRate { get; private set; }
        public CommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }

        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        public async Task InitializeTest()
        {
            SettingsManager.SettingsInstance.LastInstrumentTypeUsed = SelectedInstrument.Name;
            SettingsManager.Save();

            if (SelectedInstrument != null)
            {
                try
                {
                    var qaTestRun = ScreenManager.ResolveViewModel<QaTestRunInteractiveViewModel>();
                    await qaTestRun.Initialize(SelectedInstrument, new MechanicalDrive());
                    await ScreenManager.ChangeScreen(qaTestRun);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            VerifySettings();
        }

        private void VerifySettings()
        {
            InstrumentCommPortName = SettingsManager.SettingsInstance.InstrumentCommPort;
            BaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            TachCommPortName = SettingsManager.SettingsInstance.TachCommPort;

            NotifyOfPropertyChange(() => InstrumentCommPortName);
            NotifyOfPropertyChange(() => BaudRate);
            NotifyOfPropertyChange(() => TachCommPortName);

            if (string.IsNullOrEmpty(InstrumentCommPortName))
                ScreenManager.ShowWindow(new SettingsViewModel(ScreenManager, EventAggregator));
        }

        public class SelectableInstrumentType
        {
            public InstrumentType Instrument { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}