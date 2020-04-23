using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Repository;
using MongoDB.Driver;
using Prover.Application.Models.EvcVerifications;

namespace MongoTesting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //new AzureMongo().Connect();

            Console.WriteLine("Hello World!");
        }
    }

    public class AzureMongo
    {
        private string connString =
                "mongodb://665b2003-0ee0-4-231-b9ee:Jwq0fxQx2aeuqqd7C3Rm4C4asScPCmIN5ivu53MhcxUVwCWoVQlvs6pny1Rd6DpVOwefPp48mlyWdoWwYL9o8g==@665b2003-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        private MongoClient _client;
        private IMongoDatabase _database;

        public AzureMongo()
        {
            _client = new MongoClient(connString);
            _database = _client.GetDatabase("EvcProver");
            //_database.CreateCollection("Verifications");

        }

        public void Connect(IEnumerable<EvcVerificationTest> tests)
        {
            //new DatabaseSeeder
            var devices = DeviceRepository.Instance;
            //var dbs = await _client.ListDatabasesAsync();
            var db = _client.GetDatabase("EvcProver");

            var col = db.GetCollection<EvcVerificationTest>("Verifications");

            col.InsertMany(tests);
        }
    }   
}
