using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.FileLoader;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications;
using Tests.Application.Services;

namespace Tests.Application.FileLoader
{
    [TestClass()]
    public class ItemFilesTests
    {
        private string _filePath = ".\\MiniMax.json";
        private ItemAndTestFile _itemFile;
        private Mock<IDeviceSessionManager> _deviceManagerMock = new Mock<IDeviceSessionManager>();
        private Mock<LiveReadCoordinator> _liveReadMock = new Mock<LiveReadCoordinator>();

        [TestInitialize]
        public async Task Init()
        {
            _itemFile = await ItemLoader.LoadFromFile(StorageTestsInitialize.DeviceRepo, _filePath);
        }

        [TestMethod()]
        public async Task LoadFromFileTest()
        {
            var items = await ItemLoader.LoadFromFile(StorageTestsInitialize.DeviceRepo, _filePath);
            
            Assert.IsTrue(items != null);
        }  
        
        [TestMethod()]
        public async Task FileCommunicationClientInstanceTest()
        {
            var client = new FileDeviceClient(_itemFile);
           // var manager = new Device
            //VerificationEvents.CorrectionTests.OnLiveReadStart.Publish(new LiveReadCoordinator(_deviceManagerMock.Ob));
            
            Assert.IsTrue(client != null);
        }

    }
}