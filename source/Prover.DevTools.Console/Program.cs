using Microsoft.Extensions.Logging;
using Prover.Application.Models.EvcVerifications;
using Prover.DevTools.Importer;
using Prover.Storage.MongoDb;
using System.Threading.Tasks;

namespace Prover.DevTools.Console
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var logger = DevLogger.Logger;
			logger.LogInformation("Hello World!");
			System.Console.WriteLine("Hello World!");

			var repo = new CosmosDbAsyncRepository<EvcVerificationTest>();
			await repo.Initialize();
			await DataImporter.ImportTests(repo, "C:\\Users\\mcqua\\Source\\repos\\EvcProver\\tools\\SampleData\\ExportedTests");


			System.Console.WriteLine("Press any key to exit...");
			System.Console.ReadLine();
		}
	}
}
