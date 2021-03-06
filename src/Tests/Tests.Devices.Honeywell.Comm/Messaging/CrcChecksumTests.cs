using Devices.Communications.IO;
using Devices.Honeywell.Comm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Devices.Honeywell.Comm.Messaging
{
    /// <summary>
    /// Summary description for CrcChecksumTests
    /// </summary>
    [TestClass]
    public class CrcChecksumTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public CrcChecksumTests()
        {
            // TODO: Add constructor logic here
        }

        [TestMethod]
        public void CrcChecksumTest()
        {
            var expected = "415D";
            var cmd = $"SN,33333{ControlCharacters.STX}vq03{ControlCharacters.ETX}";
            var actual = Checksum.CalcCRC(cmd);

            Assert.AreEqual(expected, actual);
        }

        private TestContext testContextInstance;

        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()] public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run [ClassCleanup()] public
        // static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test [TestInitialize()] public void
        // MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run [TestCleanup()] public void
        // MyTestCleanup() { }
    }
}