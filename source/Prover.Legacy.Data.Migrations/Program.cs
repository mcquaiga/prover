using System;
using System.Data.Entity;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Core;
using Prover.Core.Services;
using Prover.Core.Storage;

namespace Prover.Legacy.Data.Migrations
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            await Migrator.Startup("Data Source=C:\\Users\\mcqua\\Downloads\\prover_data.sdf;Persist Security Info=False;", "");
        }
    }

    class Migrator
    {
        private static string _connString;
        private IHost _host;

        Migrator()
        {
           
        }

        public static async Task<IHost> Startup(string sqlConnectionString, string outputPath)
        {
            _connString = sqlConnectionString;
            await Load();
            var host = BuildHost();

            await host.StartAsync();

            await host.Services.GetService<InstrumentStore>().Query(i => true).Take(5).ToListAsync();
            
            return host;
        }

        public static async Task Load()
        {
            var dbContext = new ProverContext(_connString);
            var store = new InstrumentStore(dbContext);
            var service = new TestRunService(store);

            var i = service.GetAllUnexported()
                   .Take(1).FirstOrDefault();
            
            
        }

        private static IHost BuildHost() => Host.CreateDefaultBuilder()
                                                .ConfigureServices(services =>
                                                {
                                                    services.AddSingleton(new ProverContext(_connString));
                                                    services.AddSingleton<InstrumentStore>();
                                                    services.AddSingleton<TestRunService>();
                                                })
                                                .Build();

        /*
         *      //Database registrations
            builder.RegisterType<ProverContext>()
                .AsSelf()
                .As<IStartable>()
                .SingleInstance();

            //Register all types of EntityWithId as ProverStore<T>
            var asm = Assembly.GetExecutingAssembly();
            var stores = asm.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(EntityWithId)) && !t.IsAbstract)
                .Select(t => typeof(ProverStore<>).MakeGenericType(new[] { t }))
                .ToList();

            builder.RegisterTypes(stores.ToArray())
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterType<KeyValueStore>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<InstrumentStore>()
                .AsSelf()
                .As<IProverStore<Instrument>>()
                .InstancePerDependency();

            builder.RegisterType<TestRunService>()
               .SingleInstance();

            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<CertificateService>().As<ICertificateService>();
         * */
         

    }
}
