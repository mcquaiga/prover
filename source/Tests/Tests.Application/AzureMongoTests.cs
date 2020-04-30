//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MongoTesting;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Devices.Core.Repository;
//using LiteDB;
//using MongoDB.Driver;
//using Prover.Domain.EvcVerifications;
//using Prover.Infrastructure;

//namespace MongoTesting.Tests
//{
//    [TestClass()]
//    public class AzureMongoTests
//    {
//        [TestMethod()]
//        public void AzureMongoTest()
//        {
//            var dataGenerator = new DatabaseSeeder();

//            var tests = dataGenerator.CreateTests(5);
//            var az = new MongoTests();
//            az.Insert(tests);

//            var myTests = az.Select();
//        }

//        [TestMethod()]
//        public void ConnectTest()
//        {
//            Assert.Fail();
//        }
//    }

    
//    internal class MongoTests
//    {
//        private string connString =
//                "mongodb://665b2003-0ee0-4-231-b9ee:Jwq0fxQx2aeuqqd7C3Rm4C4asScPCmIN5ivu53MhcxUVwCWoVQlvs6pny1Rd6DpVOwefPp48mlyWdoWwYL9o8g==@665b2003-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

//        private MongoClient _client;
//        private IMongoDatabase _database;
//        private IMongoCollection<EvcVerificationTest> _col;

//        public MongoTests()
//        {
//            _client = new MongoClient(connString);
//            _database = _client.GetDatabase("EvcProver");
//            //_database.CreateCollection("Verifications");
//            _col = _database.GetCollection<EvcVerificationTest>("Verifications");
//        }

//        public void Insert(IEnumerable<EvcVerificationTest> tests)
//        {
//            //new DatabaseSeeder
//            var devices = DeviceRepository.Instance;
//            //var dbs = await _client.ListDatabasesAsync();

//            _col.InsertMany(tests);
//        }

//        public IEnumerable<EvcVerificationTest> Select()
//        {
//            var results = _col.Find(x => x.Verified == false).Limit(5).ToList();
//            return results;
//        }
//    }   
//}