using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Client.Desktop.Wpf.Reports;
using Devices.Core.Interfaces;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class VerificationGridViewModel : ReactiveObject
    {
        private CompositeDisposable _cleanup = new CompositeDisposable();
        public VerificationGridViewModel(
            EvcVerificationTest verificationTest,
            IVerificationTestService viewModelService,
            EvcVerificationTestService verificationTestService,
            VerificationTestReportGenerator reportService,
            ILoginService<EmployeeDTO> loginService,
            IExportVerificationTest exporter,
            ExporterViewModel exporterViewModel)
        {
            LoginService = loginService;
            ExporterViewModel = exporterViewModel;
            IsLoggedOnObservable = loginService.LoggedIn;
            Test = verificationTest;

            DeviceInfo = new DeviceInfoViewModel(verificationTest.Device);

            SetupRx();

            void SetupRx()
            {
                PrintReport = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var viewModel = await viewModelService.GetVerificationTest(Test);
                        await reportService.GenerateAndViewReport(viewModel);
                    }).DisposeWith(_cleanup);

                var canAddUser = loginService.LoggedIn;
                
                AddSignedOnUser = ReactiveCommand.CreateFromTask(async () =>
                {
                    if (loginService.User != null)
                    {
                        Test.EmployeeId = loginService.User?.Id;
                        await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return loginService.User?.Id;
                }, canAddUser).DisposeWith(_cleanup);
                canAddUser.Subscribe();

                AddSignedOnUser
                    .ToPropertyEx(this, x => x.EmployeeId, Test.EmployeeId).DisposeWith(_cleanup);


                var canAddJobId = this.WhenAnyValue(x => x.ExportedDateTime, x => x.ArchivedDateTime, (ex, a) => ex == null && a == null);
                AddJobId = ReactiveCommand.CreateFromTask(async () =>
                {
                    var jobId = await MessageInteractions.GetInputString.Handle("Enter Job #");
                    if (!string.IsNullOrEmpty(jobId))
                    {
                        Test.JobId = jobId;
                        await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return jobId;
                }, canAddJobId).DisposeWith(_cleanup);
                
                AddJobId
                    .ToPropertyEx(this, x => x.JobId, Test.JobId).DisposeWith(_cleanup);


                var canExport = this.WhenAnyValue(x => x.JobId, x => x.EmployeeId, (j, e) => !string.IsNullOrEmpty(j) && !string.IsNullOrEmpty(e));
                ExportVerification = ReactiveCommand.CreateFromTask(async () =>
                {
                    var success = await exporter.Export(Test);
                    
                    return Test.ExportedDateTime;
                }, canExport).DisposeWith(_cleanup);
                
                ExportVerification
                    .ToPropertyEx(this, x => x.ExportedDateTime, Test.ExportedDateTime, deferSubscription: true).DisposeWith(_cleanup);


                ArchiveVerification = ReactiveCommand.CreateFromTask(async () =>
                {
                    if (
                        await MessageInteractions.ShowYesNo.Handle(
                            "Are you sure you want to archive this test?"))
                    {
                        Test.ArchivedDateTime = DateTime.Now;
                        var updated = await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return Test.ArchivedDateTime;
                }).DisposeWith(_cleanup);
                
                ArchiveVerification
                    .ToPropertyEx(this, x => x.ArchivedDateTime, Test.ArchivedDateTime, deferSubscription: true).DisposeWith(_cleanup);

            }
        }

        public ILoginService<EmployeeDTO> LoginService { get; set; }
        public ExporterViewModel ExporterViewModel { get; }
        public IObservable<bool> IsLoggedOnObservable { get; set; }

        public ReactiveCommand<Unit, Unit> PrintReport { get; protected set; }
        public ReactiveCommand<Unit, string> AddSignedOnUser { get; protected set; }
        public ReactiveCommand<Unit, string> AddJobId { get; protected set; }
        public ReactiveCommand<Unit, DateTime?> ArchiveVerification { get; protected set; }
        public ReactiveCommand<Unit, DateTime?> ExportVerification { get; protected set; }

        public EvcVerificationTest Test { get; }

        public extern string EmployeeId { [ObservableAsProperty] get; }

        public extern string JobId { [ObservableAsProperty] get; }

        public extern DateTime? ExportedDateTime { [ObservableAsProperty] get; }
        public extern DateTime? ArchivedDateTime { [ObservableAsProperty] get; }

        public DeviceInfoViewModel DeviceInfo { get; }

        public string CompositionType => Test.Device.CompositionShort();

        private void SetupRx()
        {
        }
    }
}