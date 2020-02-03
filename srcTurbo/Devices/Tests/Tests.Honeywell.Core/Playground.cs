using System;
using System.Linq;
using System.Threading.Tasks;
using Devices.Honeywell.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Honeywell.Core
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var devices = HoneywellDeviceRepository.Devices.GetAll();

            var mi = devices.FirstOrDefault();

            var i = mi.Factory.CreateInstance();
        }
    }
}
