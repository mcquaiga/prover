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
            var test = TachometerCommunication.ParseTachValue(@"@D0
OK
13479");
        }
    }
}
