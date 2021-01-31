﻿using Newtonsoft.Json;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.UnionGas.Models;
using Prover.Modules.UnionGas.Verifications;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Reports;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Prover.Application.ViewModels;

namespace Prover.Modules.UnionGas.Exporter.Views
{
	public class ExportToolbarViewModel : ViewModelBase
	{
		private readonly string VsCode = "C:\\Users\\mcqua\\AppData\\Local\\Programs\\Microsoft VS Code\\code.exe";
		private readonly ILoginService<Employee> _loginService;

		public ExportToolbarViewModel
				(
				IScreenManager screenManager,
				IVerificationService verificationTestService,
				ILoginService<Employee> loginService,
				IExportVerificationTest exporter,
				VerificationTestReportGenerator reportService,
				MeterInventoryNumberValidator inventoryNumberValidator, ReadOnlyObservableCollection<EvcVerificationTest> selectedObservable = null)
		{
			_loginService = loginService;

			SetCanExecutes(this.WhenAnyValue(x => x.Selected).Where(s => s != null));

			//this.WhenAnyValue(x => x.Selected).Where(s => s != null)
			//    .ObserveOn(RxApp.MainThreadScheduler)
			//    .Subscribe();

			AddSignedOnUser = ReactiveCommand.CreateFromObservable<EvcVerificationTest, EvcVerificationTest>(test =>
											 {
												 return Observable.FromAsync(async () =>
												 {
													 test.EmployeeId = _loginService.User?.UserId;
													 return await verificationTestService.Save(test);
												 });
											 }, CanAddUser, RxApp.MainThreadScheduler)
											 .DisposeWith(Cleanup);



			AddJobId = ReactiveCommand.CreateFromObservable<EvcVerificationTest, EvcVerificationTest>(test =>
									  {
										  return Observable.FromAsync(async () =>
										  {
											  var meterDto = await inventoryNumberValidator.ValidateInventoryNumber(test);

											  if (meterDto != null)
											  {
												  test.JobId = meterDto.JobNumber.ToString();
												  test = await verificationTestService.Save(test);
											  }

											  return test;
										  });
									  }, CanAddJobId)
									  .DisposeWith(Cleanup);

			//_canExportTest = this.
			ExportVerification = ReactiveCommand.CreateFromObservable<EvcVerificationTest, EvcVerificationTest>(test =>
												{
													return Observable.FromAsync(async () =>
													{
														await exporter.Export(test);
														return test;
													});
												}, CanExport)
												.DisposeWith(Cleanup);

			ArchiveVerification = ReactiveCommand.CreateFromObservable<EvcVerificationTest, EvcVerificationTest>(test =>
												 {
													 return Observable.FromAsync(async () =>
													 {
														 if (await Messages.ShowYesNo.Handle("Are you sure you want to archive this test?"))
														 {
															 test.Archived = DateTime.Now;
															 await verificationTestService.Save(test);
														 }

														 return test;
													 });
												 }, CanArchive)
												 .DisposeWith(Cleanup);

			Updates = this.WhenAnyObservable(x => x.AddSignedOnUser, x => x.AddJobId, x => x.ArchiveVerification, x => x.ExportVerification);

			PrintReport = ReactiveCommand.CreateFromTask<EvcVerificationTest>(test =>
										 {
											 return reportService.GenerateAndViewReport(test);
											 //if (test == null)
											 // return Task.CompletedTask;

											 //var content = test.ToViewModel();

											 //return screenManager.ChangeView<ReportViewModel>((ReactiveObject)content);
										 })
										 .DisposeWith(Cleanup);

			DumpTestToJson = ReactiveCommand.CreateFromTask<EvcVerificationTest>(async test =>
			{
				var json = JsonConvert.SerializeObject(test, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					Formatting = Formatting.Indented
				});

				var filePath = GetTempFilePath();
				File.WriteAllText(filePath, json);

				Process.Start(VsCode, $@"{filePath}");

			});
		}


		//public EvcVerificationProxy VerificationProxy { get; protected set; }
		public IObservable<EvcVerificationTest> Updates { get; set; }
		public IObservable<EvcVerificationTest> SelectedItem { get; } = new Subject<EvcVerificationTest>();

		[Reactive] public EvcVerificationTest Selected { get; set; }

		public extern EvcVerificationTest VerificationTest { [ObservableAsProperty] get; }

		public IObservable<bool> CanEdit { get; set; }
		public IObservable<bool> CanAddUser { get; protected set; }
		public IObservable<bool> CanAddJobId { get; protected set; }
		public IObservable<bool> CanArchive { get; protected set; }
		public IObservable<bool> CanExport { get; protected set; }

		public ReactiveCommand<EvcVerificationTest, Unit> PrintReport { get; protected set; }
		public ReactiveCommand<EvcVerificationTest, EvcVerificationTest> AddSignedOnUser { get; }
		public ReactiveCommand<EvcVerificationTest, EvcVerificationTest> AddJobId { get; }
		public ReactiveCommand<EvcVerificationTest, EvcVerificationTest> ArchiveVerification { get; }
		public ReactiveCommand<EvcVerificationTest, EvcVerificationTest> ExportVerification { get; }

		public ReactiveCommand<EvcVerificationTest, Unit> DumpTestToJson { get; }

		public void SetCanExecutes(IObservable<EvcVerificationTest> selectedTest)
		{
			CanEdit = selectedTest.Select(t => !t.Archived.HasValue && !t.ExportedDateTime.HasValue).ObserveOn(RxApp.MainThreadScheduler);

			CanAddUser = selectedTest.Select(t => string.IsNullOrEmpty(t.EmployeeId))
									 .CombineLatest(CanEdit, (hasNoId, canEdit) => hasNoId && canEdit)
									 .CombineLatest(_loginService.LoggedIn, (noEmployeeId, isLoggedIn) => noEmployeeId && isLoggedIn)
									 .ObserveOn(RxApp.MainThreadScheduler);

			CanAddJobId = selectedTest.Select(t => string.IsNullOrEmpty(t.JobId)).CombineLatest(CanEdit, (hasNoId, canEdit) => canEdit && hasNoId).ObserveOn(RxApp.MainThreadScheduler);
			CanArchive = CanEdit;

			CanExport = this.WhenAnyObservable(x => x.CanAddUser, x => x.CanAddJobId, x => x.CanEdit, (canAddUser, canAddJob, canEdit) => !canAddUser && !canAddJob && canEdit)
							.ObserveOn(RxApp.MainThreadScheduler);
		}

		private string GetTempFilePath()
		{
			//var fileSaveDialog = new SaveFileDialog { DefaultExt = ".json", AddExtension = true };

			//if (fileSaveDialog.ShowDialog() == DialogResult.OK && fileSaveDialog.CheckPathExists == true)
			//{
			//	filePath = fileSaveDialog.FileName;
			//	return new JsonTextWriter(new StreamWriter(fileSaveDialog.OpenFile()));
			//}

			return Environment.ExpandEnvironmentVariables($"%temp%\\{Guid.NewGuid()}.json");

			//return new FileStream(filePath);
		}
	}
}

//public void SetCanExecutes(IObservable<IChangeSet<EvcVerificationTest>> selectedTests)
//{
//    CanAddUser = selectedTests.AutoRefreshOnObservable(e => _loginService.LoggedIn)
//                              .ToCollection()
//                              .Select(tests => tests.Any() && tests.All(test => string.IsNullOrEmpty(test.EmployeeId)))
//                              .CombineLatest(_loginService.LoggedIn, (noEmployeeId, isLoggedIn) => noEmployeeId && isLoggedIn)
//                              .ObserveOn(RxApp.MainThreadScheduler);

//    CanAddJobId = selectedTests.AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
//                               .ToCollection()
//                               .Select(tests => tests.Any() && tests.All(y => string.IsNullOrEmpty(y.JobId)));

//    CanArchive = selectedTests.AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
//                              .ToCollection()
//                              .Select(tests => tests.Any() && tests.All(y => !y.ArchivedDateTime.HasValue && !y.ExportedDateTime.HasValue));

//    CanExport = selectedTests.AutoRefreshOnObservable(e => SelectedObservable.ToObservable())
//                             .ToCollection()
//                             .Select(tests => tests.Any() && tests.All(test
//                                     => !test.ExportedDateTime.HasValue && !test.ArchivedDateTime.HasValue && !string.IsNullOrEmpty(test.JobId) && !string.IsNullOrEmpty(test.EmployeeId)));
//}

//private void SetCanExecutes(EvcVerificationTest evcVerification = null)
//{
//    if (evcVerification == null) return;

//    this.WhenAnyObservable(x => x.AddSignedOnUser, x => x.AddJobId, x => x.ArchiveVerification, x => x.ExportVerification)
//        .ToPropertyEx(this, x => x.VerificationTest, evcVerification);

//    CanAddUser = this.WhenAnyValue(x => x.VerificationTest)
//                     .Select(t => string.IsNullOrEmpty(t.EmployeeId))
//                     .CombineLatest(_loginService.LoggedIn.ObserveOn(RxApp.MainThreadScheduler), (b1, b2) => b1 && b2);

//    CanAddJobId = this.WhenAnyValue(x => x.VerificationTest)
//                      .Select(t => string.IsNullOrEmpty(t.JobId));

//    CanArchive = this.WhenAnyValue(x => x.VerificationTest)
//                     .Select(x => !x.ArchivedDateTime.HasValue && !x.ExportedDateTime.HasValue);

//    CanExport = this.WhenAnyValue(x => x.VerificationTest)
//                    .Select(x => !x.ExportedDateTime.HasValue && !string.IsNullOrEmpty(x.JobId) && !x.ArchivedDateTime.HasValue);
//}

