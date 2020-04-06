using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class VerificationGridViewModel : ViewModelBase
    {
        public VerificationGridViewModel(
            ILogger<VerificationGridViewModel> logger,
            EvcVerificationTest verificationTest,
            IVerificationTestService verificationTestService,
            ILoginService<EmployeeDTO> loginService,
            IExportVerificationTest exporter,
            ExporterViewModel exporterViewModel) : base(logger)
        {
            LoginService = loginService;
            ExporterViewModel = exporterViewModel;
            IsLoggedOnObservable = loginService.LoggedIn;
            Test = verificationTest;

            DeviceInfo = new DeviceInfoViewModel(verificationTest.Device);

            SetupRx();

            void SetupRx()
            {
                var canAddUser = loginService.LoggedIn.ObserveOn(RxApp.MainThreadScheduler);
                canAddUser
                    .LogDebug(x => $"CanAddUser = {x}");

                AddSignedOnUser = ReactiveCommand.CreateFromTask(async () =>
                {
                    if (loginService.User != null)
                    {
                        Test.EmployeeId = loginService.User?.Id;
                        await verificationTestService.AddOrUpdate(Test);
                    }

                    return loginService.User?.Id;
                }, canAddUser, RxApp.MainThreadScheduler).DisposeWith(Cleanup);
                //canAddUser.DefaultIfEmpty(loginService.IsSignedOn).Subscribe();

                AddSignedOnUser
                    .ToPropertyEx(this, x => x.EmployeeId, Test.EmployeeId).DisposeWith(Cleanup);


                var canAddJobId = this.WhenAnyValue(x => x.ExportedDateTime, x => x.ArchivedDateTime,
                    (ex, a) => ex == null && a == null);
                AddJobId = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var jobId = await MessageInteractions.GetInputString.Handle("Enter Job #");
                        if (!string.IsNullOrEmpty(jobId))
                        {
                            Test.JobId = jobId;
                            await verificationTestService.AddOrUpdate(Test);
                        }

                        return jobId;
                    }, canAddJobId)
                    .DisposeWith(Cleanup);

                AddJobId
                    .ToPropertyEx(this, x => x.JobId, Test.JobId)
                    .DisposeWith(Cleanup);


                var canExport = this.WhenAnyValue(x => x.JobId, x => x.EmployeeId,
                    (j, e) => !string.IsNullOrEmpty(j) && !string.IsNullOrEmpty(e));
                ExportVerification = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var success = await exporter.Export(Test);

                        return Test.ExportedDateTime;
                    }, canExport)
                    .DisposeWith(Cleanup);

                ExportVerification
                    .ToPropertyEx(this, x => x.ExportedDateTime, Test.ExportedDateTime, true)
                    .DisposeWith(Cleanup);


                ArchiveVerification = ReactiveCommand.CreateFromTask(async () =>
                {
                    if (
                        await MessageInteractions.ShowYesNo.Handle(
                            "Are you sure you want to archive this test?"))
                    {
                        Test.ArchivedDateTime = DateTime.Now;
                        var updated = await verificationTestService.AddOrUpdate(Test);
                    }

                    return Test.ArchivedDateTime;
                }).DisposeWith(Cleanup);

                ArchiveVerification
                    .ToPropertyEx(this, x => x.ArchivedDateTime, Test.ArchivedDateTime, true)
                    .DisposeWith(Cleanup);
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
    }
}