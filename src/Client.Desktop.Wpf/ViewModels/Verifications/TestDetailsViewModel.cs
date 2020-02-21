using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Application.Settings;
using Application.ViewModels;
using Application.ViewModels.Volume;
using Client.Wpf.Communications;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.ViewModels.Verifications
{
    public class TestDetailsViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IScreenManager _screenManager;

        public TestDetailsViewModel(IScreenManager screenManager, VerificationTestManager testManager)
        {
            TestManager = testManager;
            EvcVerification = TestManager.TestViewModel;
            _screenManager = screenManager;

            SaveCommand = ReactiveCommand.CreateFromTask(() => TestManager.SaveCurrentState());
            DownloadCommand = ReactiveCommand.CreateFromTask<VerificationTestPointViewModel>(test => TestManager.DownloadItems(test));

            BuildChildViewModels();
        }

        public TestDetailsViewModel(IScreenManager screenManager, EvcVerificationViewModel evcViewModel)
        {
            EvcVerification = evcViewModel;
            _screenManager = screenManager;

            SaveCommand = ReactiveCommand.CreateFromTask(() => TestManager.SaveCurrentState());
            //SaveCommand = ReactiveCommand.Create(() => );

            BuildChildViewModels();
        }

        public VerificationTestManager TestManager { get; protected set; }

        private void BuildChildViewModels()
        {
            //EvcVerification.Tests
            //    .Select(p => new CorrectionTestPointsViewModel(p))
            //    .AsObservableChangeSet()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(out var testPoints)
            //    .Subscribe();
            //TestPoints = testPoints;
        }

        [Reactive] public VolumeViewModel VolumeViewModel { get; set; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        public ReactiveCommand<VerificationTestPointViewModel, Unit> DownloadCommand { get; protected set; }

        public ReadOnlyObservableCollection<VerificationTestPointViewModel> TestPoints { get; set; }

        [Reactive] public EvcVerificationViewModel EvcVerification { get; set; }

        #region IRoutableViewModel Members

        public string UrlPathSegment => "/VerificationTests/Details";
        public IScreen HostScreen => _screenManager;

        #endregion
    }
}