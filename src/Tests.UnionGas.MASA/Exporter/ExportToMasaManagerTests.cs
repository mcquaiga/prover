using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Core.Login;
using Prover.Core.Services;
using UnionGas.MASA;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;

namespace Tests.UnionGas.MASA.Exporter
{
    [TestClass()]
    public class ExportToMasaManagerTests
    {
        private Mock<TestRunService> _testRunService;
        private Mock<DCRWebServiceCommunicator> _webService;
        private Mock<ILoginService<EmployeeDTO>> _loginService;
        private ExportToMasaManager _exportManager;

        [TestInitialize]
        public async Task Init()
        {
            _testRunService = new Mock<TestRunService>();
            _webService = new Mock<DCRWebServiceCommunicator>();
            _loginService = new Mock<ILoginService<EmployeeDTO>>();

            _exportManager = new ExportToMasaManager(_testRunService.Object,_loginService.Object, _webService.Object);
        }

        [TestMethod()]
        public void ExportToMasaManagerTest()
        {
            
        }

        [TestMethod()]
        public void ExportTest()
        {
            
        }

        [TestMethod()]
        public void ExportTest1()
        {
            
        }

        [TestMethod()]
        public void ExportFailedTestTest()
        {
            
        }
    }
}