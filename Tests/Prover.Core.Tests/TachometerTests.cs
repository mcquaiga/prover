using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Core.Communication;

namespace Prover.Core.Tests
{
    [TestClass]
    public class TachometerTests
    {
        [TestMethod]
        public void TestParseTachometer()
        {
            var value = "D0" + Environment.NewLine + "OK" + Environment.NewLine + "10000";
            var test = TachometerCommunication.ParseTachValue(value);
        }
    }
}
