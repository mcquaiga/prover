using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Enums;
using Prover.Storage.EF.Repositories;

namespace Prover.Storage.EF.Tests.Repositories
{
    [TestFixture]
    public class VerificationRunRepositoryTests : PersistenceTest<VerificationRun>
    {
        [SetUp]
        public void Init()
        {
        }

        protected override EfRepository<VerificationRun> SetRepository()
        {
            return new VerificationRunRepository(GetContext());
        }

        [Test]
        public async Task Can_save_and_load_test_run()
        {
            var testRun = this.GetTestVerificationRun();
            testRun.TestPoints = this.GetTestVerificationTestPoint();

            var dbTestRun = await SaveAndLoadEntity(testRun);

            Assert.IsNotNull(dbTestRun);
            Assert.IsTrue(dbTestRun.Id != Guid.Empty);
            dbTestRun.PropertiesShouldEqual(testRun, "TestDateTime");
            Assert.IsNotNull(dbTestRun.Instrument);
            dbTestRun.Instrument.PropertiesShouldEqual(testRun.Instrument);
            Assert.IsNotEmpty(dbTestRun.TestPoints);
        }

        [Test]
        public async Task Can_update_test_run()
        {
            var testRun = this.GetTestVerificationRun();
            var dbRun = await SaveAndLoadEntity(testRun);

            var now = DateTime.UtcNow;
            testRun.ExportedDateTime = now;
            dbRun.ExportedDateTime = now;
            var entities = Repository.Table.ToList();
            dbRun = await SaveAndLoadEntity(dbRun);
            dbRun.PropertiesShouldEqual(testRun, "TestDateTime");
        }
    }
}