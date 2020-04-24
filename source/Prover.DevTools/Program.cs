using Microsoft.Extensions.Logging;
using Prover.Application.Models.EvcVerifications;
using Prover.DevTools.Importer;
using Prover.Storage.MongoDb;
using System;
using System.Threading.Tasks;

namespace Prover.DevTools
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var logger = DevLogger.Logger;
			logger.LogInformation("Hello World!");
			Console.WriteLine("Hello World!");

			var repo = new CosmosDbAsyncRepository<EvcVerificationTest>();
			await repo.Initialize();
			await DataImporter.ImportTests(repo, "C:\\Users\\mcqua\\Source\\repos\\EvcProver\\tools\\SampleData\\ExportedTests");


			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}
	}
}
