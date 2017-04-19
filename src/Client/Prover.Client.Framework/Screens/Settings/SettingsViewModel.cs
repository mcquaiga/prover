using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Client.Framework.Settings;
using ReactiveUI;

namespace Prover.Client.Framework.Screens.Settings
{
    public class SettingsViewModel : ViewModelBase, IWindowSettings
    {
        private ReactiveList<MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits =
            new ReactiveList<MechanicalUncorrectedTestLimit>();

        public SettingsViewModel(IScreenManager screenManager)
            : base(screenManager)
        {
            MechanicalUncorrectedTestLimits.AddRange(
                SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits.ToList());
        }

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
            //EventAggregator.PublishOnUIThread(new SettingsChangeEvent());
            base.CanClose(callback);
        }

        public async Task SaveSettings()
        {
            SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits =
                MechanicalUncorrectedTestLimits.ToList();
            await SettingsManager.Save();
        }
    }
}