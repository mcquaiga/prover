using System;
using System.Windows.Forms;
using Prover.Core.Startup;

namespace Prover.DebugTools
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            var boot = new CoreBootstrapper();
        }
    }
}