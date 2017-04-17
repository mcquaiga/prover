using System;
using System.Threading.Tasks;

namespace Prover.InstrumentProtocol.Core.Extensions
{
    public static class TaskExtensions
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception)
            {
                // log errors
            }
        }
    }
}