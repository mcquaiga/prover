using System;
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
    public class SettingsViewModel : ViewModelBase, IWindowSettings
    {
        public SettingsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        { 
            MechanicalUncorrectedTestLimits.AddRange(SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits.ToList());            
        }

        public async Task SaveSettings()
        {
            SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits =
                        MechanicalUncorrectedTestLimits.ToList();
            await SettingsManager.Save();
        }

        private ReactiveList<MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits = new ReactiveList<MechanicalUncorrectedTestLimit>();
        public ReactiveList<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits
        {
            get { return _mechanicalUncorrectedTestLimits; }
            set { this.RaiseAndSetIfChanged(ref _mechanicalUncorrectedTestLimits, value); }
        }

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.CanResizeWithGrip;
                settings.MinWidth = 600;
                settings.Title = "Settings";
                return settings;
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            Task.Run(async () => await SaveSettings());
            EventAggregator.PublishOnUIThreadAsync(new SettingsChangeEvent());
            base.CanClose(callback);
        }      
    }
}