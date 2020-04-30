using System.ServiceProcess;

namespace Prover.Updater.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new UpdaterService()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
