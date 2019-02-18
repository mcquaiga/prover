namespace UnionGas.MASA.Validators.CompanyNumber
{
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
    using UnionGas.MASA.DCRWebService;
    using UnionGas.MASA.Dialogs.CompanyNumberDialog;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// Defines the <see cref="CompanyNumberValidationManager" />
    /// </summary>
    public class CompanyNumberValidationManager : IPreTestValidation
    {
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
        private readonly DCRWebServiceSoap _webService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyNumberValidationManager"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
        /// <param name="webService">The webService<see cref="DCRWebServiceSoap"/></param>
        /// <param name="loginService">The loginService<see cref="ILoginService{EmployeeDTO}"/></param>
        public CompanyNumberValidationManager(ScreenManager screenManager, TestRunService testRunService, DCRWebServiceSoap webService, ILoginService<EmployeeDTO> loginService)
        {
            _screenManager = screenManager;
            _testRunService = testRunService;
            _webService = webService;
            _loginService = loginService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Validate
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task Validate(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.RawValue.TrimStart('0');

            var serialNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.SerialNumber);
            var serialNumber = serialNumberItem.RawValue.TrimStart('0');

            MeterDTO meterDto;
            do
            {
                meterDto = await ValidateInstrumentExistsOnOpenJob(companyNumber);

                if (string.IsNullOrEmpty(meterDto?.InventoryCode) || string.IsNullOrEmpty(meterDto?.SerialNumber)
                    || meterDto.InventoryCode != companyNumber || meterDto.SerialNumber.TrimStart('0') != serialNumber)
                {
                    _log.Warn($"Inventory number {companyNumber} not found in an open job.");
                    companyNumber = (string)await Update(commClient, instrument, new CancellationTokenSource().Token);
                }
                else
                {
                    break;
                }

            } while (!string.IsNullOrEmpty(companyNumber));

            if (meterDto != null)
                await UpdateInstrumentValues(instrument, meterDto);
        }

        /// <summary>
        /// The OpenCompanyNumberDialog
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        private string OpenCompanyNumberDialog()
        {
            while (true)
            {
                var dialog = _screenManager.ResolveViewModel<CompanyNumberDialogViewModel>();
                var result = _screenManager.ShowDialog(dialog);

                if (result.HasValue && result.Value)
                {
                    _log.Debug($"New company number {dialog.CompanyNumber} was entered.");
                    if (string.IsNullOrEmpty(dialog.CompanyNumber))
                        continue;

                    return dialog.CompanyNumber;
                }

                _log.Debug($"Skipping inventory code verification.");
                return string.Empty;
            }
        }

        /// <summary>
        /// The Update
        /// </summary>
        /// <param name="evcCommunicationClient">The evcCommunicationClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        private async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument, CancellationToken ct)
        {
            var newCompanyNumber = OpenCompanyNumberDialog();
            if (string.IsNullOrEmpty(newCompanyNumber)) return string.Empty;

            await evcCommunicationClient.Connect(ct);
            var response =
                await
                    evcCommunicationClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));

            await evcCommunicationClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber;
                await _testRunService.Save(instrument);
            }

            return newCompanyNumber;
        }

        /// <summary>
        /// The UpdateInstrumentValues
        /// </summary>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="meterDto">The meterDto<see cref="MeterDTO"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task UpdateInstrumentValues(Instrument instrument, MeterDTO meterDto)
        {
            instrument.JobId = meterDto?.JobNumber.ToString();
            instrument.EmployeeId = _loginService.User?.Id;
            await _testRunService.Save(instrument);
        }

        /// <summary>
        /// The VerifyWithWebService
        /// </summary>
        /// <param name="companyNumber">The companyNumber<see cref="string"/></param>
        /// <returns>The <see cref="Task{MeterDTO}"/></returns>
        public async Task<MeterDTO> ValidateInstrumentExistsOnOpenJob(string companyNumber)
        {
            _log.Debug($"Verifying inventory number {companyNumber} with MASA.");

            try
            {
                var tokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 0, 3));
                tokenSource.Token.ThrowIfCancellationRequested();

                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };

                var response = await Task.Run(async () => await _webService.GetValidatedEvcDeviceByInventoryCodeAsync(request), tokenSource.Token).ConfigureAwait(false);

                return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
            }
            catch (OperationCanceledException)
            {
                _log.Warn($"Timed out contacting the web service. Skipping company number verification.");
                return null;
            }
            catch (EndpointNotFoundException)
            {
                _log.Warn($"Web service not available. Skipping company number verification.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"An error occured contacting the web service. Skipping company number verification.");
                return null;
            }
        }

        #endregion
    }
}
