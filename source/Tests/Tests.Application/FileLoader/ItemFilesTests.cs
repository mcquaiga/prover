using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.FileLoader;
using Tests.Application.Services;

namespace Tests.Application.FileLoader
{
    [TestClass()]
    public class ItemFilesTests
    {
        [TestMethod()]
        public async Task LoadFromFileTest()
        {
            var items = await ItemLoader.LoadFromFile(StorageTestsInitialize.DeviceRepo, ".\\MiniMax.json");
            
            Assert.IsTrue(items != null);
        }
    }
}