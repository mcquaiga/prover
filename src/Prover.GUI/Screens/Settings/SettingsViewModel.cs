using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Prover.Core.Events;
using Prover.Core.Settings;
using ReactiveUI;

namespace Prover.GUI.Screens.Settings
{
    public class SettingsViewModel : ViewModelBase, INavigationItem
    {
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, ISettingsService settingsService)
            : base(screenManager, eventAggregator)
        {
            _settingsService = settingsService;

            StabilizeLiveReadings = _settingsService.Shared.TestSettings.StabilizeLiveReadings;
            this.WhenAnyValue(x => x.StabilizeLiveReadings)
                .Subscribe(x => _settingsService.Shared.TestSettings.StabilizeLiveReadings = x);

            SelectedMechanicalVolumeTestType = _settingsService.Shared.TestSettings.MechanicalDriveVolumeTestType.ToString();
            this.WhenAnyValue(x => x.SelectedMechanicalVolumeTestType)
                .Subscribe(x => _settingsService.Shared.TestSettings.MechanicalDriveVolumeTestType =
                        (TestSettings.VolumeTestType) Enum.Parse(typeof(TestSettings.VolumeTestType), x));

            MechanicalUncorrectedTestLimits
                .AddRange(_settingsService.Shared.TestSettings.MechanicalUncorrectedTestLimits.ToList());
            this.WhenAnyValue(x => x.MechanicalUncorrectedTestLimits)
                .Subscribe(x =>
                    _settingsService.Shared.TestSettings.MechanicalUncorrectedTestLimits = x.ToList());

            SaveSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _settingsService.SaveSharedSettings();
            });
        }

        #region Commands

        public ReactiveCommand SaveSettingsCommand { get; }

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

        public LocalSettings LocalSettings => _settingsService.Local;
        public SharedSettings SharedSettings => _settingsService.Shared;

        private bool _stabilizeLiveReadings;
        public bool StabilizeLiveReadings
        {
            get => _stabilizeLiveReadings;
            set => this.RaiseAndSetIfChanged(ref _stabilizeLiveReadings, value);
        }

        private ReactiveList<MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits =
            new ReactiveList<MechanicalUncorrectedTestLimit>();

        public ReactiveList<MechanicalUncorrectedTestLimit> MechanicalUncorrectedTestLimits
        {
            get => _mechanicalUncorrectedTestLimits;
            set => this.RaiseAndSetIfChanged(ref _mechanicalUncorrectedTestLimits, value);
        }

        #endregion

        protected override void OnActivate()
        {
            _settingsService.RefreshSettings();
            base.OnActivate();
        }

        public override void CanClose(Action<bool> callback)
        {
            EventAggregator.PublishOnUIThreadAsync(new SettingsChangeEvent());
            base.CanClose(callback);
        }

        public ReactiveCommand<Unit, Unit> NavigationCommand => ReactiveCommand.Create(() => ScreenManager.ChangeScreen(this));
        public PackIconKind IconKind => PackIconKind.Settings;
        public bool IsHome => false;
    }
}