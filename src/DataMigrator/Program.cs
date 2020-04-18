using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Legacy.Data.Migrations;

namespace DataMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Migrator.Startup("Data Source=C:\\Users\\mcqua\\Downloads\\prover_data.sdf;Persist Security Info=False;", "");
        }
    }

    class Migrator
    {
        private static string _connString;
        // private IHost _host;

        Migrator()
        {

        }

        public static async Task Startup(string sqlConnectionString, string outputPath)
        {
            _connString = sqlConnectionString;
            await Load();
            //var host = BuildHost();

            //await host.StartAsync();

            //await host.Services.GetService<InstrumentStore>().Query(i => true).Take(5).ToListAsync();

            //return host;
        }

        public static async Task Load()
        {
            var dbContext = new MigrateContext();
            // (dbContext as IStartable).Start();
            var store = new InstrumentStore(dbContext);
            //var service = new TestRunService(store);

            // var i = store.Get(Guid.Parse("449d8df2-4472-4f1a-9a8f-1d9ba651b32b"));
            var jsonSettings = new JsonSerializerSettings()
            {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
            };

            var myTests = store.Query(i => i.Type == 4)
                               .OrderBy(x => x.TestDateTime)
                               .Skip(150)
                               .Take(100)
                               .ToList();
            var count = 0;
            myTests.ForEach(async test =>
            {
                count++;
                using (var writer = new StreamWriter($".\\ExportedTests\\{test.Id}.json"))
                {
                    try
                    {
                        //var includedTest = store.Get(test.Id);
                        var qa = await Translator.ToQaTestRun(test);

                        var json = JsonConvert.SerializeObject(qa, jsonSettings);
                        await writer.WriteAsync(json);
                        Debug.WriteLine($"{count} of {myTests.Count}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex}");
                    }
                }
            });

            //var tests = store.Query().Where(x => x.InstrumentType.Id == 4)
            //                 .Take(10)
            //                 .AsEnumerable()
            //                 .ToList();

            //var mini = DataTransfer.GetDevice(4);
            //var t = Translator.ToQaTestRun(i);
        }
    }

    public static class Translator
    {
        public static async Task<QaTestRunDTO> ToQaTestRun(Instrument from)
        {
            var items = from.Items.ToDictionary(iv => iv.Metadata.Number.ToString(), iv => iv.RawValue);

            var tests = from.VerificationTests.ToDictionary(k => k.TestNumber,
                    vt => new List<ItemValue>().Concat(vt.PressureTest?.Items ?? new List<ItemValue>())
                                               .Concat(vt.TemperatureTest?.Items ?? new List<ItemValue>())
                                               .Concat(vt.VolumeTest?.Items ?? new List<ItemValue>())
                                               //.Concat(vt.SuperFactorTest?.Items ?? new List<ItemValue>())
                                              
                                               .ItemValuesToDictionary());

            var volume = from.VerificationTests.FirstOrDefault(x => x.VolumeTest != null)
                             ?.VolumeTest.AfterTestItems.ItemValuesToDictionary();


            var qaTest = new QaTestRunDTO(DataTransfer.GetDeviceTypeId(from.Type), items)
            {
                    Tests = tests.Select(x => new TestDTO() {TestNumber = x.Key, Values = x.Value})
                                 .ToList(),
                    IsPassed = from.HasPassed,
                    TestDateTime =  from.TestDateTime
            };

            qaTest.Tests.First(t => t.TestNumber == 0)
                  .EndValues = volume;
            
            return qaTest;
        }

        public static Dictionary<string, string> ItemValuesToDictionary(this IEnumerable<ItemValue> values)
        {
            return values.ToDictionary(iv => iv.Metadata.Number.ToString(), iv => iv.RawValue);
        }
    }
}
