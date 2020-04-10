using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.VerificationEvents;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExportToolbarViewModel : ViewModelBase
    {
        private readonly ILoginService<EmployeeDTO> _loginService;

        public ExportToolbarViewModel(
            IScreenManager screenManager,
            IVerificationTestService verificationTestService,
            ILoginService<EmployeeDTO> loginService,
            IExportVerificationTest exporter,
            MeterInventoryNumberValidator inventoryNumberValidator,
            ReadOnlyObservableCollection<EvcVerificationTest> selectedObservable = null)
        {
            _loginService = loginService;
            SelectedObservable = selectedObservable;
            SetCanExecutes(selectedObservable.ToObservableChangeSet());

            AddSignedOnUser = ReactiveCommand
                .CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(tests =>
                    {
                        return Observable.Create<EvcVerificationTest>(async obs =>
                        {
                            tests.ForEach(t => t.EmployeeId = _loginService.User?.Id);
                            tests.ForEach(t => verificationTestService.AddOrUpdate(t));
                            tests.ForEach(obs.OnNext);
                        });
                    },
                    CanAddUser,
                    RxApp.MainThreadScheduler).DisposeWith(Cleanup);

            AddJobId = ReactiveCommand.CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(
                tests =>
                {
                    return Observable.Create<EvcVerificationTest>(async obs =>
                    {
                        var updatedTests = new List<EvcVerificationTest>();
                        //var jobId = await MessageInteractions.GetInputString.Handle("Enter Job #");
                        foreach (var evcVerificationTest in tests)
                        {
                            var meterDto = await inventoryNumberValidator.ValidateInventoryNumber(evcVerificationTest);
                            if (meterDto != null)
                            {
                                evcVerificationTest.JobId = meterDto.JobNumber.ToString();
                                updatedTests.Add(evcVerificationTest);
                            }
                        }
                        
                        updatedTests.ForEach(async t => 
                            obs.OnNext(await verificationTestService.AddOrUpdate(t)));
                    });
                }, CanAddJobId).DisposeWith(Cleanup);

            //_canExportTest = this.
            ExportVerification = ReactiveCommand
                .CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(tests =>
                {
                    return Observable.Create<EvcVerificationTest>(async obs =>
                    {
                        foreach (var evcVerificationTest in tests)
                        {
                            var success = await exporter.Export(evcVerificationTest);
                            if (success) obs.OnNext(evcVerificationTest);
                        }
                    });
                }, CanExport).DisposeWith(Cleanup);

            ArchiveVerification = ReactiveCommand
                .CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(tests =>
                {
                    return Observable.Create<EvcVerificationTest>(async obs =>
                    {
                        if (await MessageInteractions.ShowYesNo.Handle("Are you sure you want to archive this test?"))
                            foreach (var evcVerificationTest in tests)
                            {
                                evcVerificationTest.ArchivedDateTime = DateTime.Now;
                                var test = await verificationTestService.AddOrUpdate(evcVerificationTest);
                                obs.OnNext(test);
                            }
                    });
                }, CanArchive).DisposeWith(Cleanup);

            Updates = this.WhenAnyObservable(x => x.AddSignedOnUser, x => x.AddJobId, x => x.ArchiveVerification,
                x => x.ExportVerification);

            //PrintReport = ReactiveCommand.CreateFromTask<ICollection<EvcVerificationTest>>(async tests =>
            //{
            //    var test = tests.FirstOrDefault();
            //    if (test == null) return;

            //    var viewModel = await verificationTestService.GetViewModel(test);
            //    var reportViewModel = await screenManager.ChangeView<ReportViewModel>();
            //    reportViewModel.ContentViewModel = viewModel;
            //}).DisposeWith(Cleanup);
        }

        //public EvcVerificationProxy VerificationProxy { get; protected set; }
        public ICollection<EvcVerificationTest> SelectedItems => SelectedObservable;

        public IObservable<EvcVerificationTest> Updates { get; set; }

        public ReadOnlyObservableCollection<EvcVerificationTest> SelectedObservable { get; }
        public extern EvcVerificationTest VerificationTest { [ObservableAsProperty] get; }

        public IObservable<bool> CanAddUser { get; protected set; }
        public IObservable<bool> CanAddJobId { get; protected set; }
        public IObservable<bool> CanArchive { get; protected set; }
        public IObservable<bool> CanExport { get; protected set; }

        public ReactiveCommand<ICollection<EvcVerificationTest>, Unit> PrintReport { get; protected set; }

        public ReactiveCommand<ICollection<EvcVerificationTest>, EvcVerificationTest> AddSignedOnUser { get; }

        public ReactiveCommand<ICollection<EvcVerificationTest>, EvcVerificationTest> AddJobId { get; }

        public ReactiveCommand<ICollection<EvcVerificationTest>, EvcVerificationTest> ArchiveVerification { get; }

        public ReactiveCommand<ICollection<EvcVerificationTest>, EvcVerificationTest> ExportVerification { get; }

        private void SetCanExecutes(EvcVerificationTest evcVerification = null)
        {
            if (evcVerification == null) return;


            this.WhenAnyObservable(x => x.AddSignedOnUser, x => x.AddJobId, x => x.ArchiveVerification,
                    x => x.ExportVerification)
                .ToPropertyEx(this, x => x.VerificationTest, evcVerification);

            CanAddUser = this.WhenAnyValue(x => x.VerificationTest)
                .Select(t => string.IsNullOrEmpty(t.EmployeeId))
                .CombineLatest(_loginService.LoggedIn.ObserveOn(RxApp.MainThreadScheduler), (b1, b2) => b1 && b2);

            CanAddJobId = this.WhenAnyValue(x => x.VerificationTest).Select(t => string.IsNullOrEmpty(t.JobId));
            CanArchive = this.WhenAnyValue(x => x.VerificationTest)
                .Select(x => !x.ArchivedDateTime.HasValue && !x.ExportedDateTime.HasValue);
            CanExport = this.WhenAnyValue(x => x.VerificationTest).Select(x =>
                !x.ExportedDateTime.HasValue && !string.IsNullOrEmpty(x.JobId) && !x.ArchivedDateTime.HasValue);
        }

        private void SetCanExecutes(IObservable<IChangeSet<EvcVerificationTest>> selectedTests)
        {
            CanAddUser =
                selectedTests.AutoRefreshOnObservable(e => _loginService.LoggedIn)
                    .ToCollection()
                    .Select(tests => tests.Any() && tests.All(test => string.IsNullOrEmpty(test.EmployeeId)))
                    .CombineLatest(_loginService.LoggedIn,
                        (noEmployeeId, isLoggedIn) => noEmployeeId && isLoggedIn)
                    .ObserveOn(RxApp.MainThreadScheduler);

            CanAddJobId = selectedTests
                .AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
                .ToCollection()
                .Select(tests => tests.Any() && tests.All(y => string.IsNullOrEmpty(y.JobId)));

            CanArchive = selectedTests
                .AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
                .ToCollection()
                .Select(tests =>
                    tests.Any() && tests.All(y => !y.ArchivedDateTime.HasValue && !y.ExportedDateTime.HasValue));

            CanExport = selectedTests
                .AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
                .ToCollection()
                .Select(tests => tests.Any() && tests.All(
                    test => !test.ExportedDateTime.HasValue && !test.ArchivedDateTime.HasValue &&
                            !string.IsNullOrEmpty(test.JobId) &&
                            !string.IsNullOrEmpty(test.EmployeeId)));
        }
    }
}