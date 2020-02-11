using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.EntityFrameworkSqlDataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Repositories.Tests
{
    [TestClass()]
    public class EvcVerificationRepositoryTests
    {

        [TestMethod()]
        public void EvcVerificationRepositoryTest()
        {
            //var dbOptions = new DbContextOptionsBuilder<ProverDbContext>()
            //    .UseSqlite(Configuration)
            //    .Options;

            //var db = new ProverDbContext(dbOptions);
            //var evc = new EvcVerificationRepository(db);



            Assert.Fail();
        }
    }
}