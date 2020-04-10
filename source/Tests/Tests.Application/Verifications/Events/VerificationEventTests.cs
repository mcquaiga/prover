using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Verifications.Events;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Application.Verifications.Events.Tests
{
    [TestClass()]
    public class VerificationEventTests
    {
        private static VerificationEvent<string, string> _event = new VerificationEvent<string, string>();

        [TestMethod()]
        public async Task PublishTest()
        {
            _event.Subscribe(x => x.SetOutput("Bar"));

            var result = await _event.Publish("FooEvent");

            Assert.IsTrue(result == "Bar");
        }
    }
}