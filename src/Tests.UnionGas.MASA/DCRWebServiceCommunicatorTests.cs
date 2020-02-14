using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionGas.MASA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Prover.Core.Login;
using Prover.Core.Services;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;

namespace UnionGas.MASA.Tests
{
    [TestClass()]
    public class DCRWebServiceCommunicatorTests
    {
        private Mock<DCRWebServiceSoap> _soapService;
        private DCRWebServiceCommunicator _webService;


        [TestInitialize]
        public void Init()
        {
            _soapService = new Mock<DCRWebServiceSoap>();

            
        }

        [TestMethod()]
        public void DCRWebServiceCommunicatorTest()
        {
            
        }

        [TestMethod()]
        public async Task FindMeterByCompanyNumberTest()
        {
            var expectedDto = new MeterDTO()
            {
                InventoryCode = "123456",
                MeterType = "RM 2000",
                SerialNumber = "654321",
                JobNumber = 123456
            };

            var request = new GetValidatedEvcDeviceByInventoryCodeRequest
            {
                Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody("123456")
            };

            var response = new GetValidatedEvcDeviceByInventoryCodeResponse
            {
                Body = new GetValidatedEvcDeviceByInventoryCodeResponseBody(expectedDto)
            };

            _soapService.Setup(s => 
                        s.GetValidatedEvcDeviceByInventoryCodeAsync(It.IsAny<GetValidatedEvcDeviceByInventoryCodeRequest>())
                    ).ReturnsAsync(response);


            _webService = new DCRWebServiceCommunicator(_soapService.Object);

            var actualDto = await _webService.FindMeterByCompanyNumber("123456");

            Assert.AreEqual(expectedDto, actualDto);
        }

        [TestMethod()]
        public void GetEmployeeTest()
        {
           
        }

        [TestMethod()]
        public void GetOutstandingMeterTestsByJobNumberTest()
        {
           
        }

        [TestMethod()]
        public void SendQaTestResultsTest()
        {

        }
    }
}