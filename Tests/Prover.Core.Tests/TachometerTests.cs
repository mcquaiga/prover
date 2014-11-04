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

            var value = "D0" + (char) 13 + "OK" + (char) 13 + "10000";
            var test = TachometerCommunication.ParseTachValue(value);
        }
    }
}
