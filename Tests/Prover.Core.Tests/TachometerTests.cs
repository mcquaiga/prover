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
            const string value = @"D0
                                   OK
                                   9968";
            var test = TachometerCommunication.ParseTachValue(value);
        }
    }
}
