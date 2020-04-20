﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber;
using Prover.Modules.UnionGas.Models;
using Prover.Modules.UnionGas.Verifications;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ViewModelWpfBase, IRoutableViewModel, IHaveToolbarItems
    {
        private readonly ReadOnlyObservableCollection<EvcVerificationTest> _data;

        public ExporterViewModel
        (IScreenManager screenManager, IVerificationTestService verificationTestService, IEntityDataCache<EvcVerificationTest> verificationCache, IDeviceRepository deviceRepository,
                IExportVerificationTest exporter, ILoginService<Employee> loginService, TestsByJobNumberViewModel testsByJobNumberViewModel, MeterInventoryNumberValidator inventoryNumberValidator,
                Func<ExportToolbarViewModel> exporterToolbarFactory = null
        )
        {
            ScreenManager = screenManager;
            TestsByJobNumberViewModel = testsByJobNumberViewModel;
            HostScreen = screenManager;
            DeviceTypes = deviceRepository.GetAll().OrderBy(d => d.Name).ToList();
            DeviceTypes = DeviceTypes.Prepend(new AllDeviceType {Id = Guid.Empty, Name = "All"}).ToList();
            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, Func<EvcVerificationTest, bool>>(BuildDeviceFilter).DisposeWith(Cleanup);
            FilterIncludeArchived = ReactiveCommand.Create<bool, Func<EvcVerificationTest, bool>>(BuildIncludeArchivedFilter).DisposeWith(Cleanup);
            FilterIncludeExported = ReactiveCommand.Create<bool, Func<EvcVerificationTest, bool>>(BuildIncludeExportedFilter).DisposeWith(Cleanup);

            var visibleItems = verificationCache.Updates.Connect()
                                                .Filter(FilterByTypeCommand)
                                                .Filter(FilterIncludeExported.StartWith(BuildIncludeExportedFilter(false))) //, changeObservable.Select(x => Unit.Default)
                                                .Filter(FilterIncludeArchived.StartWith(BuildIncludeArchivedFilter(false)));

            visibleItems.Sort(SortExpressionComparer<EvcVerificationTest>.Ascending(t => t.TestDateTime))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Bind(out _data)
                        .DisposeMany()
                        .Subscribe()
                        .DisposeWith(Cleanup);

            verificationCache.Data().Connect()
                             .Filter(x => !string.IsNullOrEmpty(x.JobId))
                             .DistinctValues(x => x.JobId)
                             .Bind(out var jobIds).Subscribe()
                             .DisposeWith(Cleanup);
            JobIdsList = jobIds;

            ToolbarViewModel = exporterToolbarFactory?.Invoke() ?? new ExportToolbarViewModel(screenManager, verificationTestService, loginService, exporter, inventoryNumberValidator);
            //AddToolbarItem(ToolbarViewModel.ToolbarActionItems);
        
        }

        public ExportToolbarViewModel ToolbarViewModel { get; set; }
        public ReadOnlyObservableCollection<string> JobIdsList { get; set; }

        public ReactiveCommand<DeviceType, Func<EvcVerificationTest, bool>> FilterByTypeCommand { get; protected set; }
        public ReactiveCommand<bool, Func<EvcVerificationTest, bool>> FilterIncludeExported { get; protected set; }
        public ReactiveCommand<bool, Func<EvcVerificationTest, bool>> FilterIncludeArchived { get; protected set; }

        public ReadOnlyObservableCollection<EvcVerificationTest> Data => _data;

        public ICollection<DeviceType> DeviceTypes { get; }

        public IScreenManager ScreenManager { get; }
        public TestsByJobNumberViewModel TestsByJobNumberViewModel { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildDeviceFilter(DeviceType deviceType) => t => t.Device.DeviceType.Id == deviceType.Id || deviceType.Id == Guid.Empty;

        private Func<EvcVerificationTest, bool> BuildIncludeArchivedFilter(bool include) => test => include || test.ArchivedDateTime == null;

        private Func<EvcVerificationTest, bool> BuildIncludeExportedFilter(bool include) => test => include || test.ExportedDateTime == null;

        #region Nested type: AllDeviceType

        private class AllDeviceType : DeviceType
        {
            public override Type GetBaseItemGroupClass(Type itemGroupType) => throw new NotImplementedException();

            public override TGroup GetGroup<TGroup>(IEnumerable<ItemValue> itemValues) => throw new NotImplementedException();

            public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType) => throw new NotImplementedException();

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