using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Screens.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {            
            StabilizeLiveReadings = SettingsManager.SettingsInstance.TestSettings.StabilizeLiveReadings;
            this.WhenAnyValue(x => x.StabilizeLiveReadings)
                .Subscribe(x => SettingsManager.SettingsInstance.TestSettings.StabilizeLiveReadings = x);

            SelectedMechanicalVolumeTestType =
                SettingsManager.SettingsInstance.TestSettings.MechanicalDriveVolumeTestType.ToString();
            this.WhenAnyValue(x => x.SelectedMechanicalVolumeTestType)
                .Subscribe(x =>
                    SettingsManager.SettingsInstance.TestSettings.MechanicalDriveVolumeTestType = (TestSettings.VolumeTestType)Enum.Parse(typeof(TestSettings.VolumeTestType), x));
            
            MechanicalUncorrectedTestLimits
                .AddRange(SettingsManager.SettingsInstance.TestSettings.MechanicalUncorrectedTestLimits.ToList());
            this.WhenAnyValue(x => x.MechanicalUncorrectedTestLimits)
                .Subscribe(x => SettingsManager.SettingsInstance.TestSettings.MechanicalUncorrectedTestLimits = x.ToList());
            
            SaveSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SettingsManager.Save();
            });
        }
     
        #region Commands

        public ReactiveCommand SaveSettingsCommand { get; private set; }

        #endregion

        #region Reactive Properties

        public List<string> MechanicalVolumeTestType =>
            Enum.GetNames(typeof(TestSettings.VolumeTestType)).ToList();       
      
        private string _selectedMechanicalVolumeTestType;
        public string SelectedMechanicalVolumeTestType
        {
            get => _selectedMechanicalVolumeTestType;
            set => this.RaiseAndSetIfChanged(ref _selectedMechanicalVolumeTestType, value);
        }

        public Core.Settings.Settings Settings => SettingsManager.SettingsInstance;

        private bool _stabilizeLiveReadings;
        public bool StabilizeLiveReadings
        {
            get => _stabilizeLiveReadings;
            set => this.RaiseAndSetIfChanged(ref _stabilizeLiveReadings, value);
        }

        private ReactiveList<MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits = new ReactiveList<MechanicalUncorrectedTestLimit>();
        public ReactiveList<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits
        {
            get => _mechanicalUncorrectedTestLimits;
            set => this.RaiseAndSetIfChanged(ref _mechanicalUncorrectedTestLimits, value);
        }      

        #endregion
        
        public override void CanClose(Action<bool> callback)
        {           
            EventAggregator.PublishOnUIThreadAsync(new SettingsChangeEvent());
            base.CanClose(callback);
        }      
    }
}