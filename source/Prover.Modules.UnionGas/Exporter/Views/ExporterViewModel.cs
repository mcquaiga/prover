using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Client.Desktop.Wpf.Reports;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IExportVerificationTest _exporter;

        public ExporterViewModel(IScreenManager screenManager,
            EvcVerificationTestService service,
            IVerificationTestService verificationTestService,
            IDeviceRepository deviceRepository,
            VerificationTestReportGenerator reportService,
            IExportVerificationTest exporter,
            ILoginService<EmployeeDTO> loginService,
            Func<EvcVerificationTest, ExporterViewModel, VerificationGridViewModel> verificationViewModelFactory,
            TestsByJobNumberViewModel testsByJobNumberViewModel)
        {
            _exporter = exporter;
            ScreenManager = screenManager;
            TestsByJobNumberViewModel = testsByJobNumberViewModel;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.GetAll().OrderBy(d => d.Name).ToList();
            DeviceTypes = DeviceTypes.Prepend(new AllDeviceType {Id = Guid.Empty, Name = "All"}).ToList();

            PrintReport = ReactiveCommand.CreateFromTask<EvcVerificationTest>(async test =>
            {
                var viewModel = await verificationTestService.GetVerificationTest(test);
                var reportViewModel = await screenManager.ChangeView<ReportViewModel>();
                reportViewModel.ContentViewModel = viewModel;
            }).DisposeWith(Cleanup);

            FilterByTypeCommand =
                ReactiveCommand.Create<DeviceType, Func<EvcVerificationTest, bool>>(BuildDeviceFilter);

            var includeExportedFilter =
                this.WhenAnyValue(x => x.IncludeExportedTests).Select(BuildIncludeExportedFilter);
            
            var includeArchivedFilter = 
                this.WhenAnyValue(x => x.IncludeArchived).Select(BuildIncludeArchivedFilter);

            var reSorter = FilterByTypeCommand.Merge(includeExportedFilter).Merge(includeArchivedFilter)
                .Select(_ => Unit.Default);
            var sortExpression = SortExpressionComparer<VerificationGridViewModel>
                    .Ascending(t => !string.IsNullOrEmpty(t.JobId) ? t.JobId : "ZZZZZZZ")
                    .ThenByAscending(t => t.Test.TestDateTime);

            var filteredTests = service.FetchTests().Connect()
                .Filter(FilterByTypeCommand)
                .Filter(includeExportedFilter)
                .Filter(includeArchivedFilter);
            
            filteredTests
                .Transform(x => verificationViewModelFactory.Invoke(x, this))
                .Sort(sortExpression, reSorter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var visibleTests)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Cleanup);
            
            VisibleTests = visibleTests;

            filteredTests
                .Filter(x => !string.IsNullOrEmpty(x.JobId))
                //.DistinctValues(x => !string.IsNullOrEmpty(x.JobId) ? x.JobId : "")
                .DistinctValues(x => x.JobId)
                .Bind(out var jobIds)
                .Subscribe()
                .DisposeWith(Cleanup);
            
            JobIdsList = jobIds;
        }

        public ReadOnlyObservableCollection<string> JobIdsList { get; set; }

        public ReactiveCommand<EvcVerificationTest, Unit> PrintReport { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, string> AddSignedOnUser { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, DateTime?> ArchiveVerification { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, DateTime?> ExportVerification { get; protected set; }

        [Reactive] public bool IncludeExportedTests { get; set; } = false;
        [Reactive] public bool IncludeArchived { get; set; } = false;

        public ReactiveCommand<DeviceType, Func<EvcVerificationTest, bool>> FilterByTypeCommand { get; protected set; }

        public ReadOnlyObservableCollection<VerificationGridViewModel> VisibleTests { get; }

        public ICollection<DeviceType> DeviceTypes { get; } = new List<DeviceType>();

        public IScreenManager ScreenManager { get; }
        public TestsByJobNumberViewModel TestsByJobNumberViewModel { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildDeviceFilter(DeviceType deviceType) => t =>
            t.Device.DeviceType.Id == deviceType.Id || deviceType.Id == Guid.Empty;

        private Func<EvcVerificationTest, bool> BuildIncludeArchivedFilter(bool include) =>
            test => include || test.ArchivedDateTime == null;

        private Func<EvcVerificationTest, bool> BuildIncludeExportedFilter(bool include) =>
            test => include || test.ExportedDateTime == null;

        #region Nested type: AllDeviceType

        private class AllDeviceType : DeviceType
        {
            public override Type GetBaseItemGroupClass(Type itemGroupType) => throw new NotImplementedException();

            public override TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) =>
                throw new NotImplementedException();

            public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType) =>
                throw new NotImplementedException();

            public override IEnumerable<ItemMetadata> GetItemMetadata<T>() => throw new NotImplementedException();
        }

        #endregion
    }
}

/*
 * void SetupRx()
            {
                PrintReport =
                    ReactiveCommand.CreateFromTask<VerificationGridViewModel, Unit>(async (vm) =>
                    {
                        var viewModel = await verificationTestService.GetVerificationTest(vm.Test);
                        await reportService.GenerateAndViewReport(viewModel);
                        return Unit.Default;
                    });

                AddSignedOnUser = ReactiveCommand.CreateFromTask<VerificationGridViewModel, string>(async (vm) =>
                {
                    if (loginService.User != null)
                    {
                        vm.Test.EmployeeId = loginService.User?.Id;
                        await service.AddOrUpdateVerificationTest(vm.Test);
                    }

                    return loginService.User?.Id;
                }, loginService.LoggedIn);
                
                var canExport = this.WhenAnyValue(x => x.HasJobId, x => x.EmployeeId,
                    (j, e) => j && !string.IsNullOrEmpty(e));
                ExportVerification = ReactiveCommand.CreateFromTask(async (vm) =>
                {
                    var success = await exporter.Export(vm.Test);

                    if (success)
                    {
                        Test.ExportedDateTime = DateTime.Now;
                        await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return Test.ExportedDateTime;
                }, canExport);

                HasJobId = !string.IsNullOrEmpty(Test.JobId);
                ExportVerification.ToPropertyEx(this, x => x.ExportedDateTime, Test.ExportedDateTime);
                //ExportVerification
                //    .Select(x => x != null)
                //    .ToPropertyEx(this, x => x.IsExported, Test.ExportedDateTime != null);

                ArchiveVerification = ReactiveCommand.CreateFromTask(async (vm) =>
                {
                    if (
                        await MessageInteractions.ShowYesNo.Handle(
                            "Are you sure you want to archive this test?"))
                    {
                        vm.Test.ArchivedDateTime = DateTime.Now;
                        var updated = await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return Test.ArchivedDateTime;
                });
                ArchiveVerification
                    .ToPropertyEx(this, x => x.ArchivedDateTime, Test.ArchivedDateTime);
            }
 * */