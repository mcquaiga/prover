using System;
using System.Threading.Tasks;


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

    public class Migrator
    {
        private static string _connString;

        Migrator()
        {
           
        }

        public static async Task Startup(string sqlConnectionString, string outputPath)
        {
            _connString = sqlConnectionString;
            await Load();
        }

        public static async Task Load()
        {
           

            
        }
        

    }

}
