namespace UnionGas.MASA.Validators {

	using NLog;
	using Prover.CommProtocol.Common;
	using Prover.CommProtocol.Common.Items;
	using Prover.Core.Login;
	using Prover.Core.Models.Instruments;
	using Prover.Core.Services;
	using Prover.Core.VerificationTests.TestActions;
	using Prover.GUI.Screens;
	using System;
	using System.Reactive.Subjects;
	using System.ServiceModel;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using UnionGas.MASA.DCRWebService;
	using UnionGas.MASA.Dialogs.BarCodeDialog;
	using LogManager = NLog.LogManager;

	/// <summary>
	/// Defines the <see cref="BarCodeValidationManager"/>
	/// </summary>
	public class BarCodeValidationManager : IEvcDeviceValidationAction {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BarCodeValidationManager"/> class.
		/// </summary>
		/// <param name="screenManager">The screenManager <see cref="ScreenManager"/></param>
		/// <param name="testRunService">The testRunService <see cref="TestRunService"/></param>
		/// <param name="webService">The webService <see cref="DCRWebServiceSoap"/></param>
		/// <param name="loginService">The loginService <see cref="ILoginService{EmployeeDTO}"/></param>
		public BarCodeValidationManager(ScreenManager screenManager, TestRunService testRunService, DCRWebServiceCommunicator webService, ILoginService<EmployeeDTO> loginService) {
			_screenManager = screenManager;
			_testRunService = testRunService;
			_webService = webService;
			_loginService = loginService;
		}

		#endregion

		#region Properties

		public VerificationStep VerificationStep => VerificationStep.PreVerification;

		#endregion

		#region Methods

		public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null) {
			try {
				var barCode = OpenBarCodeDialog();
				MeterDTO meterDto;
				do {
					meterDto = await _webService.FindMeterByCompanyNumber(instrument.InventoryNumber, barCode);

					if (string.IsNullOrEmpty(meterDto?.InventoryCode) || string.IsNullOrEmpty(meterDto?.SerialNumber)
						|| meterDto.InventoryCode != barCode || meterDto.SerialNumber.TrimStart('0') != barCode) {
						_log.Warn($"Bar Code Number {barCode} not found in an open job.");
						barCode = (string)await Update(commClient, instrument, new CancellationTokenSource().Token);
					}
					else {
						break;
					}
				} while (!string.IsNullOrEmpty(barCode));

				if (meterDto != null) {
					await UpdateInstrumentValues(instrument, meterDto);
				}
			}
			catch (EndpointNotFoundException) {
				return;
			}
		}

		#endregion

		#region Fields

		/// <summary>
		/// Defines the _log
		/// </summary>
		private readonly Logger _log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Defines the _loginService
		/// </summary>
		private readonly ILoginService<EmployeeDTO> _loginService;

		/// <summary>
		/// Defines the _screenManager
		/// </summary>
		private readonly ScreenManager _screenManager;

		/// <summary>
		/// Defines the _testRunService
		/// </summary>
		private readonly TestRunService _testRunService;

		/// <summary>
		/// Defines the _webService
		/// </summary>
		private readonly DCRWebServiceCommunicator _webService;

		#endregion

		/// <summary>
		/// The OpenCompanyNumberDialog
		/// </summary>
		/// <returns>The <see cref="string"/></returns>
		private string OpenBarCodeDialog() {
			while (true) {
				BarCodeDialogViewModel dialog = _screenManager.ResolveViewModel<BarCodeDialogViewModel>();
				bool? result = _screenManager.ShowDialog(dialog);

				if (result.HasValue && result.Value) {
					_log.Debug($"Bar Code #{dialog.BarCodeNumber} was entered.");
					if (string.IsNullOrEmpty(dialog.BarCodeNumber)) {
						continue;
					}

					return dialog.BarCodeNumber;
				}

				_log.Debug($"Skipping Bar Code verification.");
				return string.Empty;
			}
		}

		/// <summary>
		/// The Update
		/// </summary>
		/// <param name="evcCommunicationClient">The evcCommunicationClient <see cref="EvcCommunicationClient"/></param>
		/// <param name="instrument">The instrument <see cref="Instrument"/></param>
		/// <param name="ct">The ct <see cref="CancellationToken"/></param>
		/// <returns>The <see cref="Task{object}"/></returns>
		private async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument, CancellationToken ct) {
			string newCompanyNumber = string.Empty;

			Application.Current.Dispatcher.Invoke((Action)delegate {
				newCompanyNumber = OpenBarCodeDialog();
			});

			if (string.IsNullOrEmpty(newCompanyNumber)) {
				return string.Empty;
			}

			await evcCommunicationClient.Connect(ct);
			bool response =
				await
					evcCommunicationClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));

			await evcCommunicationClient.Disconnect();

			if (response) {
				instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber;
				await _testRunService.Save(instrument);
			}

			return newCompanyNumber;
		}

		/// <summary>
		/// The UpdateInstrumentValues
		/// </summary>
		/// <param name="instrument">The instrument <see cref="Instrument"/></param>
		/// <param name="meterDto">The meterDto <see cref="MeterDTO"/></param>
		/// <returns>The <see cref="Task"/></returns>
		private async Task UpdateInstrumentValues(Instrument instrument, MeterDTO meterDto) {
			instrument.BarCodeNumber = meterDto?.BarcodeNumber;
			instrument.JobId = meterDto?.JobNumber.ToString();
			instrument.EmployeeId = _loginService.User?.Id;

			await _testRunService.Save(instrument);
		}
	}
}