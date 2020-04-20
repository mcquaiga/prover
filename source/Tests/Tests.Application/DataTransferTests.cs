using System.Threading.Tasks;
using Devices.Core.Repository;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Services;
using Prover.Application.ViewModels.Factories;
using Prover.Legacy.Data.Migrations;
using Prover.Storage.LiteDb;

namespace Tests.Application
{
    [TestClass()]
    public class DataTransferTests
    {


        [TestMethod()]
        public async Task ImportTestsTest()
        {
            var db = new LiteDatabase("C:\\Users\\mcqua\\source\\repos\\EvcProver\\build\\Debug\\prover_data.db");
            //var db = new LiteDatabase("C:\\Users\\mcqua\\AppData\\Local\\EvcProver\\prover_data.db");
            var repo = new VerificationsLiteDbRepository(db, DeviceRepository.Instance);

            var testService = new VerificationTestService(null, repo, new VerificationViewModelFactory(), null);
            await DataTransfer.ImportTests(testService, $"C:\\Users\\mcqua\\source\\repos\\EvcProver_legacy\\src\\DataMigrator\\bin\\Debug\\ExportedTests");
        }
    }
}