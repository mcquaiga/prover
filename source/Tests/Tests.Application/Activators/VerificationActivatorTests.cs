using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Interfaces;
using Prover.Application.Verifications.CustomActions;

namespace Tests.Application.Activators
{
    [TestClass()]
    public class VerificationActivatorTests
    {
     
        private Activate _test;
        private VerificationActivator<Activate> _activator;

        [TestInitialize]
        public void Init()
        {
            _test = new Activate();
           _activator = new VerificationActivator<Activate>();

           _manager = new ActivationCoordinator(new []{_activator});
        }

        [TestMethod()]
        public void VerificationActivatorTest()
        {
            _activator.AddActivationBlock(t => { t.Changed = true; });
          
            _test.Activator.Activate(_test);

            Assert.IsTrue(_test.Changed);
        }

        [TestMethod()]
        public void ActivateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeactivateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }

    public class Activate : IActivatable<Activate>
    {
        public Activate()
        {
            Activator = new VerificationActivator<Activate>();
        }
        /// <inheritdoc />
        public VerificationActivator<Activate> Activator { get; set; }

        public bool Changed { get; set; } = false;
    }
}