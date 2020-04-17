using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Legacy.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devices.Core.Repository;
using LiteDB;
using Prover.Storage.LiteDb;
using Tests.Application.Services;

namespace Prover.Legacy.Data.Migrations.Tests
{
    [TestClass()]
    public class DataTransferTests
    {


        [TestMethod()]
        public async Task ImportTestsTest()
        {
            var db = new LiteDatabase("C:\\Users\\mcqua\\source\\repos\\EvcProver\\build\\Debug\\prover_data.db");
            var repo = new VerificationsLiteDbRepository(db, DeviceRepository.Instance);
           
            //var testService = new VerificationTestService(null, repo, new VerificationViewModelFactory(), null);
            await DataTransfer.ImportTests(repo, "C:\\Users\\mcqua\\source\\repos\\EvcProver\\src\\DataMigrator\\bin\\Debug\\ExportedTests");
        }
    }
}