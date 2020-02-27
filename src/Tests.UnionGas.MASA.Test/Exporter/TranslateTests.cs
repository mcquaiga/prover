using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Exporter;

namespace Tests.UnionGas.MASA.Test.Exporter
{
    [TestClass()]
    public class TranslateTests
    {
        [TestMethod()]
        public void CreateFailedTestForExportTest()
        {
            var meterDto = new MeterDTO();
            var qaTest = Translate.CreateFailedTestForExport(meterDto);

            Assert.IsNotNull(qaTest);
        }

        [TestMethod()]
        public void CreateFailedTestForExportTest2()
        {
            var meterDto = new MeterDTO()
            {
                InventoryCode = "123456",
                MeterType = "RM 2000",
                SerialNumber = "654321",
                JobNumber = 123456
            };

            var qaTest = Translate.CreateFailedTestForExport(meterDto);

            Assert.IsNotNull(qaTest);
        }
    }
}