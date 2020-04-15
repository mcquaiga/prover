using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Devices.Core.Interfaces;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Domain.EvcVerifications;
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
            var dbContext = new ProverContext();
           // (dbContext as IStartable).Start();
            var store = new InstrumentStore(dbContext);
            //var service = new TestRunService(store);

            var i = store.Get(Guid.Parse("449d8df2-4472-4f1a-9a8f-1d9ba651b32b"));

            var myTests = store.Query()
                               .Take(150)
                               .ToList()
                               .Where(x => x.InstrumentType.Id == 4);
                                     //.ToObservable();
        
            
            
            //var tests = store.Query().Where(x => x.InstrumentType.Id == 4)
            //                 .Take(10)
            //                 .AsEnumerable()
            //                 .ToList();

            //var mini = DataTransfer.GetDevice(4);
            var t = Translator.ToQaTestRun(i);

        }
    }

    public static class Translator
    {

        public static QaTestRunDTO ToQaTestRun(Instrument from)
        {
            var items = from.Items.ToDictionary(iv => iv.Metadata.Number.ToString(), iv => iv.RawValue);
            return DataTransfer.Create(from.Type, items);
        }

    }
}
