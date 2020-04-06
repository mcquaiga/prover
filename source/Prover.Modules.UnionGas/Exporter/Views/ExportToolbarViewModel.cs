﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Client.Desktop.Wpf.Reports;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.VerificationActions;
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
            SetCanExecutesTrue(selectedObservable.ToObservableChangeSet());
            
            AddSignedOnUser = ReactiveCommand
                .CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(tests =>
                    {
                        return Observable.Create<EvcVerificationTest>(async obs =>
                        {
                            foreach (var evcVerificationTest in tests)
                                if (loginService.User != null)
                                {
                                    evcVerificationTest.EmployeeId = loginService.User?.Id;
                                    var test = await verificationTestService.AddOrUpdate(evcVerificationTest);
                                    obs.OnNext(test);
                                }
                        });
                    },
                    CanAddUser,
                    RxApp.MainThreadScheduler).DisposeWith(Cleanup);

            AddJobId = ReactiveCommand.CreateFromObservable<ICollection<EvcVerificationTest>, EvcVerificationTest>(
                tests =>
                {
                    return Observable.Create<EvcVerificationTest>(async obs =>
                    {
                        var jobId = await MessageInteractions.GetInputString.Handle("Enter Job #");
                        foreach (var evcVerificationTest in tests)
                            if (!string.IsNullOrEmpty(jobId))
                            {
                                var meterDto = await inventoryNumberValidator.Validate(evcVerificationTest);
                                if (meterDto != null)
                                {
                                    evcVerificationTest.JobId = jobId;
                                    var test = await verificationTestService.AddOrUpdate(evcVerificationTest);
                                    obs.OnNext(test);
                                }
                            }
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

            PrintReport = ReactiveCommand.CreateFromTask<ICollection<EvcVerificationTest>>(async tests =>
            {
                var test = tests.First();
                var viewModel = await verificationTestService.GetViewModel(test);
                var reportViewModel = await screenManager.ChangeView<ReportViewModel>();
                reportViewModel.ContentViewModel = viewModel;
            }).DisposeWith(Cleanup);
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
            if (evcVerification == null) {
                return;
            }


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

        private void SetCanExecutesTrue(IObservable<IChangeSet<EvcVerificationTest>> selectedTests)
        {
            CanAddUser = selectedTests.ToCollection().Select(x => x.All(y => string.IsNullOrEmpty(y.EmployeeId)))
                .CombineLatest(_loginService.LoggedIn.ObserveOn(RxApp.MainThreadScheduler), (b1, b2) => b1 && b2); ;

            CanAddJobId = selectedTests.ToCollection().Select(x => x.All(y => string.IsNullOrEmpty(y.JobId)));

            CanArchive = selectedTests.ToCollection().Select(x => x.All(y => !y.ArchivedDateTime.HasValue));

            CanExport = selectedTests.ToCollection().Select(x => x.All(y => !y.ExportedDateTime.HasValue));
        }
    }
}