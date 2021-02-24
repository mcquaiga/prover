namespace UnionGas.MASA.Screens.Exporter {
	using Caliburn.Micro;
	using Prover.Core.ExternalIntegrations;
	using Prover.Core.Login;
	using Prover.Core.Models.Instruments;
	using Prover.Core.Models.Instruments.DriveTypes;
	using Prover.Core.Services;
	using Prover.GUI.Reports;
	using Prover.GUI.Screens;
	using ReactiveUI;
	using System;
	using System.Reactive;
	using System.Threading.Tasks;
	using System.Windows;
	using UnionGas.MASA.DCRWebService;

	/// <summary>
	/// Defines the <see cref="QaTestRunGridViewModel" />
	/// </summary>
	public class QaTestRunGridViewModel : ViewModelBase {
		/// <summary>
		/// Gets the AddCurrentUserCommand
		/// </summary>
		public ReactiveCommand<Unit, Unit> AddCurrentUserCommand { get; private set; }

		/// <summary>
		/// Gets or sets the ArchiveTestCommand
		/// </summary>
		public ReactiveCommand ArchiveTestCommand
		{
			get { return _archiveTestCommand; }
			set { this.RaiseAndSetIfChanged(ref _archiveTestCommand, value); }
		}

		/// <summary>
		/// Gets the DateTimePretty
		/// </summary>
		public string DateTimePretty => $"{Instrument.TestDateTime:g}";


		public string DriveDescription
		{
			get {
				if (Instrument.VolumeTest.DriveType is RotaryDrive rt) {
					return rt.Meter.MeterTypeDescription;
				}

				//if (Instrument.VolumeTest.DriveType is MechanicalDrive)
				//{
				//    return (Instrument.VolumeTest.DriveType as MechanicalDrive).Discriminator;
				//}
				return "";
			}
		}

		//if (Instrument.VolumeTest.DriveType is MechanicalDrive)
		//{
		//    return (Instrument.VolumeTest.DriveType as MechanicalDrive).Discriminator;
		//}
		/// <summary>
		/// Gets the ExportQaTestRunCommand
		/// </summary>
		public ReactiveCommand ExportQaTestRunCommand { get; }

		/// <summary>
		/// Gets or sets the Instrument
		/// </summary>
		public Instrument Instrument
		{
			get { return _instrument; }
			set { this.RaiseAndSetIfChanged(ref _instrument, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether IsRemoved
		/// </summary>
		public bool IsRemoved { get => _isRemoved; set => this.RaiseAndSetIfChanged(ref _isRemoved, value); }

		/// <summary>
		/// Gets or sets a value indicating whether IsSelected
		/// </summary>
		public bool IsSelected { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether IsShowing
		/// </summary>
		public bool IsShowing { get => _isShowing; set => this.RaiseAndSetIfChanged(ref _isShowing, value); }

		/// <summary>
		/// Gets or sets the ViewQaTestReportCommand
		/// </summary>
		public ReactiveCommand ViewQaTestReportCommand
		{
			get { return _viewQaTestReportCommand; }
			set { this.RaiseAndSetIfChanged(ref _viewQaTestReportCommand, value); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QaTestRunGridViewModel"/> class.
		/// </summary>
		/// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
		/// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
		/// <param name="exportManager">The exportManager<see cref="IExportTestRun"/></param>
		/// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
		/// <param name="instrumentReportGenerator">The instrumentReportGenerator<see cref="InstrumentReportGenerator"/></param>
		/// <param name="loginService">The loginService<see cref="ILoginService{EmployeeDTO}"/></param>
		public QaTestRunGridViewModel(ScreenManager screenManager,
				IEventAggregator eventAggregator,
				IExportTestRun exportManager,
				TestRunService testRunService,
				InstrumentReportGenerator instrumentReportGenerator,
				ILoginService<EmployeeDTO> loginService)
			: base(screenManager, eventAggregator) {
			_exportManager = exportManager;
			_testRunService = testRunService;
			_instrumentReportGenerator = instrumentReportGenerator;
			_loginService = loginService;

			//var canAddUser = this.WhenAnyValue(x => x._loginService.User, dto => !string.IsNullOrEmpty(dto?.Id));
			AddCurrentUserCommand = ReactiveCommand.CreateFromTask(async () => {
				if (!loginService.IsLoggedIn)
					await loginService.GetLoginDetails();

				if (_loginService.IsLoggedIn) {
					Instrument.EmployeeId = _loginService.User.Id;
					await testRunService.Save(Instrument);
					this.RaisePropertyChanged($"Instrument");
				}

			});

			var canExport = this.WhenAnyValue(x => x.Instrument.JobId, x => x.Instrument.EmployeeId,
				(jobId, employeeId) => !string.IsNullOrEmpty(jobId) && !string.IsNullOrEmpty(employeeId));

			ExportQaTestRunCommand = ReactiveCommand.CreateFromTask(ExportQaTestRun, canExport);

			ExportQaTestRunCommand
				.ThrownExceptions
				.Subscribe(ex => {
					MessageBox.Show("An error occured exporting the test. See logs for more details");
					Log.Error(ex.InnerException);
				});

			ArchiveTestCommand = ReactiveCommand.CreateFromTask(ArchiveTest);

			ViewQaTestReportCommand = ReactiveCommand.CreateFromTask(DisplayInstrumentReport);
		}

		/// <summary>
		/// The ArchiveTest
		/// </summary>
		/// <returns>The <see cref="Task"/></returns>
		public async Task ArchiveTest() {
			await _testRunService.ArchiveTest(Instrument);
			IsRemoved = true;
		}

		/// <summary>
		/// The DisplayInstrumentReport
		/// </summary>
		/// <returns>The <see cref="Task"/></returns>
		public async Task DisplayInstrumentReport() {
			await _instrumentReportGenerator.GenerateAndViewReport(Instrument);
		}

		/// <summary>
		/// The ExportQaTestRun
		/// </summary>
		/// <returns>The <see cref="Task"/></returns>
		public async Task ExportQaTestRun() {
			if (string.IsNullOrEmpty(Instrument.JobId) || string.IsNullOrEmpty(Instrument.EmployeeId))
				return;

			IsRemoved = await _exportManager.Export(Instrument);
		}

		/// <summary>
		/// The Initialize
		/// </summary>
		/// <param name="instrument">The instrument<see cref="Instrument"/></param>
		/// <param name="filterObservable">The filterObservable<see cref="IObservable{Predicate{Instrument}}"/></param>
		public void Initialize(Instrument instrument, IObservable<Predicate<Instrument>> filterObservable) {
			Instrument = instrument;

			SetFilter(filterObservable);
		}

		/// <summary>
		/// The SetFilter
		/// </summary>
		/// <param name="filter">The filter<see cref="IObservable{Predicate{Instrument}}"/></param>
		public void SetFilter(IObservable<Predicate<Instrument>> filter) {
			filter.Subscribe(x => {
				IsShowing = x(Instrument);
			});
		}

		/// <summary>
		/// Defines the _exportManager
		/// </summary>
		private readonly IExportTestRun _exportManager;

		/// <summary>
		/// Defines the _instrumentReportGenerator
		/// </summary>
		private readonly InstrumentReportGenerator _instrumentReportGenerator;

		/// <summary>
		/// Defines the _loginService
		/// </summary>
		private readonly ILoginService<EmployeeDTO> _loginService;

		/// <summary>
		/// Defines the _testRunService
		/// </summary>
		private readonly TestRunService _testRunService;

		/// <summary>
		/// Defines the _archiveTestCommand
		/// </summary>
		private ReactiveCommand _archiveTestCommand;

		/// <summary>
		/// Defines the _instrument
		/// </summary>
		private Instrument _instrument;

		/// <summary>
		/// Defines the _isRemoved
		/// </summary>
		private bool _isRemoved;

		/// <summary>
		/// Defines the _isShowing
		/// </summary>
		private bool _isShowing;

		/// <summary>
		/// Defines the _viewQaTestReportCommand
		/// </summary>
		private ReactiveCommand _viewQaTestReportCommand;

		/// <summary>
		/// The AddCurrentUserToTest
		/// </summary>
		/// <returns>The <see cref="Task"/></returns>
		private async Task AddCurrentUserToTest() {
			if (!_loginService.IsLoggedIn) {
				await _loginService.GetLoginDetails();
			}

			if (_loginService.IsLoggedIn) {
				Instrument.EmployeeId = _loginService.User.Id;
				await _testRunService.Save(Instrument);
				this.RaisePropertyChanged($"Instrument");
			}
		}
	}
}