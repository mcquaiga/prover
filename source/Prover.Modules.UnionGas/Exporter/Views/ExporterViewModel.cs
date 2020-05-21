using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using DynamicData;
using DynamicData.Binding;
using MaterialDesignThemes.Wpf;
using Prover.Application.Extensions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber;
using Prover.Modules.UnionGas.Models;
using Prover.Modules.UnionGas.Verifications;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Prover.Application.Services;
using Prover.Application.Specifications;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.UnionGas.Exporter.Views
{
	public class ExporterViewModel : ViewModelBase, IRoutableViewModel, IToolbarButton
	{
		private readonly IQueryableRepository<EvcVerificationTest> _verificationRepository;
		private readonly ReadOnlyObservableCollection<EvcVerificationTest> _data;

		public ExporterViewModel(
				IScreenManager screenManager,
				IVerificationService verificationService,
				IQueryableRepository<EvcVerificationTest> verificationRepository,
				IEntityCache<EvcVerificationTest> verificationCache,
				IDeviceRepository deviceRepository,
				IExportVerificationTest exporter,
				ILoginService<Employee> loginService,
				TestsByJobNumberViewModel testsByJobNumberViewModel,
				MeterInventoryNumberValidator inventoryNumberValidator,
				ExportToolbarViewModel exportToolbar)
		{
			_verificationRepository = verificationRepository;
			ScreenManager = screenManager;
			TestsByJobNumberViewModel = testsByJobNumberViewModel;
			HostScreen = screenManager;

			OpenCommand = ReactiveCommand.CreateFromTask(async () => { await screenManager.ChangeView<ExporterViewModel>(); });

			DeviceTypes = deviceRepository.GetAll()
				.OrderBy(d => d.Name).Prepend(new AllDeviceType { Id = Guid.Empty, Name = "All" })
										.ToList();

			FilterByTypeCommand = ReactiveCommand.Create<DeviceType, Func<EvcVerificationTest, bool>>(BuildDeviceFilter).DisposeWith(Cleanup);
			TestDateFilter = ReactiveCommand.Create(DateTimeFilter).DisposeWith(Cleanup);
			FilterIncludeArchived = ReactiveCommand.Create<bool, Func<EvcVerificationTest, bool>>(BuildIncludeArchivedFilter).DisposeWith(Cleanup);
			FilterIncludeExported = ReactiveCommand.Create<bool, Func<EvcVerificationTest, bool>>(BuildIncludeExportedFilter).DisposeWith(Cleanup);

			FilterByTypeCommand.ThrownExceptions
							   .Merge(TestDateFilter.ThrownExceptions)
							   .Merge(FilterIncludeArchived.ThrownExceptions)
							   .Merge(FilterIncludeExported.ThrownExceptions)
							   .LogErrors(Logger)
							   .Subscribe()
							   .DisposeWith(Cleanup);

			ReloadData = ReactiveCommand.CreateFromObservable(() => verificationRepository.QueryObservable(VerificationQuerySpecs.Default));

			var visibleItems = verificationCache.Data.Connect()
												.Merge(ReloadData.ToObservableChangeSet(k => k.Id))
												.Filter(FilterByTypeCommand)
												.Filter(FilterIncludeExported.StartWith(BuildIncludeExportedFilter(false))) //, changeObservable.Select(x => Unit.Default)
												.Filter(FilterIncludeArchived.StartWith(BuildIncludeArchivedFilter(false)))
												.LogErrors(Logger);

			visibleItems.Sort(SortExpressionComparer<EvcVerificationTest>.Ascending(t => t.TestDateTime))
						.ObserveOn(RxApp.MainThreadScheduler)
						.Bind(out _data)
						.DisposeMany()
						.Subscribe()
						.DisposeWith(Cleanup);

			ToolbarViewModel = exportToolbar; //; exporterToolbarFactory?.Invoke() ?? new ExportToolbarViewModel(screenManager, verificationTestService, loginService, exporter, inventoryNumberValidator);
											  //AddToolbarItem(ToolbarViewModel.ToolbarActionItems);
		}

		//public ReactiveCommand<Unit, IChangeSet<EvcVerificationTest, Guid>> ReloadData { get; set; }
		public ReactiveCommand<Unit, EvcVerificationTest> ReloadData { get; set; }

		public ExportToolbarViewModel ToolbarViewModel { get; set; }
		//public ReadOnlyObservableCollection<string> JobIdsList { get; set; }

		public ReactiveCommand<DeviceType, Func<EvcVerificationTest, bool>> FilterByTypeCommand { get; protected set; }
		public ReactiveCommand<Unit, Func<EvcVerificationTest, bool>> TestDateFilter { get; private set; }
		public ReactiveCommand<bool, Func<EvcVerificationTest, bool>> FilterIncludeExported { get; protected set; }
		public ReactiveCommand<bool, Func<EvcVerificationTest, bool>> FilterIncludeArchived { get; protected set; }

		public ReadOnlyObservableCollection<EvcVerificationTest> Data => _data;

		public ICollection<DeviceType> DeviceTypes { get; }

		public IScreenManager ScreenManager { get; }
		public TestsByJobNumberViewModel TestsByJobNumberViewModel { get; }
		public string UrlPathSegment => "/Exporter";
		public IScreen HostScreen { get; }

		[Reactive] public DateTime FromDateTime { get; set; } = DateTime.Today.AddDays(-30);
		[Reactive] public DateTime ToDateTime { get; set; } = DateTime.Today.AddDays(1);

		private Func<EvcVerificationTest, bool> BuildDeviceFilter(DeviceType deviceType)
		{
			return t => t.Device.DeviceType.Id == deviceType.Id || deviceType.Id == Guid.Empty;
		}

		private Func<EvcVerificationTest, bool> BuildIncludeArchivedFilter(bool include)
		{
			return test => include || test.ArchivedDateTime == null;
		}

		private Func<EvcVerificationTest, bool> BuildIncludeExportedFilter(bool include)
		{
			return test => include || test.ExportedDateTime == null;
		}

		private Func<EvcVerificationTest, bool> DateTimeFilter()
		{
			return test => test.TestDateTime.Between(FromDateTime, ToDateTime);
		}

		private Expression<Func<EvcVerificationTest, bool>> DateTimePredicate()
		{
			return test => test.TestDateTime.Between(FromDateTime, ToDateTime);
		}
		public PackIconKind MenuIconKind { get; } = PackIconKind.CloudUpload;
		public string MenuTitle { get; } = "Export Test Run";
		public ReactiveCommand<Unit, Unit> OpenCommand { get; }

		#region Nested type: AllDeviceType

		private class AllDeviceType : DeviceType
		{
			public override Type GetBaseItemGroupClass(Type itemGroupType)
			{
				throw new NotImplementedException();
			}

			public override TGroup GetGroup<TGroup>(IEnumerable<ItemValue> itemValues)
			{
				throw new NotImplementedException();
			}

			public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType)
			{
				throw new NotImplementedException();
			}

			public override IEnumerable<ItemMetadata> GetItemMetadata<T>()
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		/// <inheritdoc />
		public int SortOrder { get; } = 50;

		/// <inheritdoc />
		public ToolbarItemType ItemType { get; } = ToolbarItemType.MainMenu;

		/// <inheritdoc />
		public string Icon { get; } = PackIconKind.ClipboardList.ToString();

		/// <inheritdoc />
		public ICommand ToolbarAction => OpenCommand;
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
