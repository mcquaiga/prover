using Prover.CommProtocol.MiHoneywell.Items;
using System.Threading.Tasks;

namespace Prover.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dev = new DeviceTypeLoader();
            Task.Run(() => dev.LoadDevicesAsync());
        }
    }
}
