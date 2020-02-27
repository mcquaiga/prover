using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Shared.Data;
using UnionGas.MASA;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;

namespace UnionGas.MASATests.Exporter
{
    [TestClass()]
    public class ExportToMasaManagerTests
    {
        private Mock<IProverStore<Instrument>> _proverStoreMock;
        private Mock<TestRunService> _testRunService;
        private DCRWebServiceCommunicator _webService;
        private Mock<ILoginService<EmployeeDTO>> _loginService;
        private Mock<DCRWebServiceSoap> _dcrWebService;
        private ExportToMasaManager _exportManager;

        private EmployeeDTO _employeeDto = new EmployeeDTO(){EmployeeNbr = "12345", EmployeeName = "Adam", Id = "54321"};

        [TestInitialize]
        public void Init()
        {
            _proverStoreMock = new Mock<IProverStore<Instrument>>();
            _testRunService = new Mock<TestRunService>(_proverStoreMock.Object);

            _dcrWebService = new Mock<DCRWebServiceSoap>(MockBehavior.Loose);
            _webService = new DCRWebServiceCommunicator(_dcrWebService.Object);

            _loginService = new Mock<ILoginService<EmployeeDTO>>();
            _loginService.Setup(l => l.User).Returns(_employeeDto);
        }

        [TestMethod()]
        public async Task ExportToMasaManagerTest()
        {
            var inventory = "1234567";
            var meter = new MeterDTO()
            {
                InventoryCode = inventory
            };
            var response = new GetValidatedEvcDeviceByInventoryCodeResponse(new GetValidatedEvcDeviceByInventoryCodeResponseBody(meter));
            var response2 = new SubmitQAEvcTestResultsResponse(new SubmitQAEvcTestResultsResponseBody("success"));
            _dcrWebService.Setup(service => service.GetValidatedEvcDeviceByInventoryCodeAsync(It.IsAny<GetValidatedEvcDeviceByInventoryCodeRequest>()))
                .ReturnsAsync(response);

            _dcrWebService.Setup(service =>
                    service.SubmitQAEvcTestResultsAsync(It.IsAny<SubmitQAEvcTestResultsRequest>()))
                .ReturnsAsync(response2);

            _exportManager = new ExportToMasaManager(_testRunService.Object, _loginService.Object, _webService);

            var success = await _exportManager.ExportFailedTest(inventory);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ExportTest()
        {
            
        }

        [TestMethod()]
        public void ExportTest1()
        {
            
        }

    }
}