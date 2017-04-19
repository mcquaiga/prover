using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Caliburn.Micro;
using Prover.ProtocolMonitor.ViewModel;
using Splat;
using Action = System.Action;
using LogManager = NLog.LogManager;

namespace ProtocolMonitor.UI
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();
            Locator.CurrentMutable.Register(LogManager.GetCurrentClassLogger, typeof(NLog.ILogger));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var ass = base.SelectAssemblies().ToList();
            ass.Add(Assembly.Load("Prover.ProtocolMonitor"));
            return ass;
        }
    }
}